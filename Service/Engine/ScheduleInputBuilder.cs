using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Engine.Solver;
using Shared.DTOs.Engine;

namespace Service.Engine;

public sealed class ScheduleInputBuilder
{ 
    public async Task<InputBuilderResult> BuildAsync(
        ScheduleSolveRequest request,
        IRepository<CourseOffering> courseOfferingRepository,
        IRepository<Professor> professorRepository,
        IRepository<TimeSlot> timeSlotRepository,
        IRepository<Course> courseRepository,
        CancellationToken ct = default)
    {
        var offeringsQuery = courseOfferingRepository.Table
            .Include(x => x.Course)
                .ThenInclude(x => x.PreRequisites)
            .Include(x => x.CombinedCourseGroups)
            .Where(x => x.TermId == request.TermId);

        if (request.UniversityId.HasValue)
            offeringsQuery = offeringsQuery.Where(x => x.UniversityId == request.UniversityId.Value);

        var offerings = await offeringsQuery.ToListAsync(ct);

        if (offerings.Count == 0)
            return InputBuilderResult.Failed("No course offerings found for the selected term.");

        var universityIds = offerings
            .Select(x => x.UniversityId)
            .Distinct()
            .ToList();

        var courseIds = offerings
            .Select(x => x.CourseId)
            .Distinct()
            .ToList();

        var professors = await professorRepository.TableNoTracking
            .Include(x => x.Skills)
            .Include(x => x.Availabilities)
            .Where(p => p.Skills.Any(s => courseIds.Contains(s.CourseId)))
            .ToListAsync(ct);

        if (professors.Count == 0)
            return InputBuilderResult.Failed("No professors with matching skills found.");

        var timeSlots = await timeSlotRepository.TableNoTracking
            .Where(x => universityIds.Contains(x.UniversityId))
            .ToListAsync(ct);

        if (timeSlots.Count == 0)
            return InputBuilderResult.Failed("No time slots found for selected universities.");

        var allCourses = await courseRepository.TableNoTracking
            .Include(x => x.PreRequisites)
            .ToListAsync(ct);

        var unitsResult = BuildSchedulingUnits(offerings);

        if (!unitsResult.IsSuccess)
            return InputBuilderResult.Failed(unitsResult.ErrorMessage);

        var prerequisiteClosure = BuildPrerequisiteClosure(allCourses);

        var input = new ScheduleInput
        {
            Units = unitsResult.Units,
            Professors = professors.Select(p => new ProfessorSolverDto
            {
                Id = p.Id,
                MaxWeeklyMinutes = p.MaxWeeklyMinutes,
                Skills = p.Skills.Select(s => new ProfessorSkillSolverDto
                {
                    CourseId = s.CourseId,
                    Priority = s.Priority,
                    SkillLevel = s.SkillLevel
                }).ToList(),
                Availabilities = p.Availabilities.Select(a => new ProfessorAvailabilitySolverDto
                {
                    UniversityId = a.UniversityId,
                    DayOfWeek = a.DayOfWeek,
                    StartMinutes = a.StartMinutes,
                    EndMinutes = a.EndMinutes
                }).ToList()
            }).ToList(),

            TimeSlots = timeSlots.Select(ts => new TimeSlotSolverDto
            {
                Id = ts.Id,
                UniversityId = ts.UniversityId,
                DayOfWeek = ts.DayOfWeek,
                StartMinutes = ToMinutes(ts.StartTime),
                EndMinutes = ToMinutes(ts.EndTime)
            }).ToList(),

            PrerequisiteClosure = prerequisiteClosure,
            DefaultRoomId = request.DefaultRoomId,
            TimeLimitSeconds = request.TimeLimitSeconds
        };

        return InputBuilderResult.Success(input);
    }

    private static int ToMinutes(TimeOnly time)
    {
        return time.Hour * 60 + time.Minute;
    }

    private static BuildUnitsResult BuildSchedulingUnits(List<CourseOffering> offerings)
    {
        var result = new List<SchedulingUnit>();

        var usedOfferingIds = new HashSet<long>();

        var groupedOfferings = offerings
            .Where(x => x.CombinedCourseGroups.Count > 0)
            .GroupBy(x => x.CombinedCourseGroups.Single().Id)
            .ToList();

        foreach (var group in groupedOfferings)
        {
            var groupOfferings = group.ToList();

            foreach (var offering in groupOfferings)
            {
                if (!usedOfferingIds.Add(offering.Id))
                {
                    return BuildUnitsResult.Failed(
                        $"CourseOffering {offering.Id} is assigned to more than one combined group.");
                }
            }

            var sessionCounts = groupOfferings
                .Select(x => x.Course.WeeklySessionCount)
                .Distinct()
                .ToList();

            if (sessionCounts.Count != 1)
            {
                return BuildUnitsResult.Failed(
                    $"Combined group {group.Key} has offerings with different WeeklySessionCount.");
            }

            var durations = groupOfferings
                .Select(x => x.Course.SessionDurationMinutes)
                .Distinct()
                .ToList();

            if (durations.Count != 1)
            {
                return BuildUnitsResult.Failed(
                    $"Combined group {group.Key} has offerings with different SessionDurationMinutes.");
            }

            var universityIds = groupOfferings
                .Select(x => x.UniversityId)
                .Distinct()
                .ToList();

            if (universityIds.Count != 1)
            {
                return BuildUnitsResult.Failed(
                    $"Combined group {group.Key} contains offerings from different universities.");
            }

            result.Add(new SchedulingUnit
            {
                UnitKey = $"G:{group.Key}",
                WeeklySessionCount = sessionCounts.Single(),
                SessionDurationMinutes = durations.Single(),
                UniversityId = universityIds.Single(),
                CourseIds = groupOfferings.Select(x => x.CourseId).ToHashSet(),
                Offerings = groupOfferings.Select(x => new UnitOffering
                {
                    CourseOfferingId = x.Id,
                    CourseId = x.CourseId,
                    StudentCount = x.StudentCount
                }).ToList()
            });
        }

        var standaloneOfferings = offerings
            .Where(x => !usedOfferingIds.Contains(x.Id))
            .ToList();

        foreach (var offering in standaloneOfferings)
        {
            result.Add(new SchedulingUnit
            {
                UnitKey = $"O:{offering.Id}",
                WeeklySessionCount = offering.Course.WeeklySessionCount,
                SessionDurationMinutes = offering.Course.SessionDurationMinutes,
                UniversityId = offering.UniversityId,
                CourseIds = [offering.CourseId],
                Offerings =
                [
                    new UnitOffering
                    {
                        CourseOfferingId = offering.Id,
                        CourseId = offering.CourseId,
                        StudentCount = offering.StudentCount
                    }
                ]
            });
        }

        return BuildUnitsResult.Success(result);
    }

    private static Dictionary<long, HashSet<long>> BuildPrerequisiteClosure(List<Course> courses)
    {
        var byId = courses.ToDictionary(x => x.Id);

        var directMap = courses.ToDictionary(
            x => x.Id,
            x => x.PreRequisites.Select(p => p.Id).ToList());

        var result = new Dictionary<long, HashSet<long>>();

        foreach (var course in courses)
        {
            var visited = new HashSet<long>();
            CollectPrerequisites(course.Id, directMap, visited);
            result[course.Id] = visited;
        }

        return result;
    }

    private static void CollectPrerequisites(
        long courseId,
        Dictionary<long, List<long>> directMap,
        HashSet<long> visited)
    {
        if (!directMap.TryGetValue(courseId, out var directPrerequisites))
            return;

        foreach (var prerequisiteId in directPrerequisites)
        {
            if (!visited.Add(prerequisiteId))
                continue;

            CollectPrerequisites(prerequisiteId, directMap, visited);
        }
    }

    private sealed class BuildUnitsResult
    {
        public bool IsSuccess { get; init; }
        public string ErrorMessage { get; init; } = "";
        public List<SchedulingUnit> Units { get; init; } = [];

        public static BuildUnitsResult Success(List<SchedulingUnit> units)
        {
            return new BuildUnitsResult
            {
                IsSuccess = true,
                Units = units
            };
        }

        public static BuildUnitsResult Failed(string message)
        {
            return new BuildUnitsResult
            {
                IsSuccess = false,
                ErrorMessage = message
            };
        }
    }
}
