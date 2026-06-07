using Service.Engine.Solver;
using Shared.DTOs.Engine;

namespace Service.Engine;

using Google.OrTools.Sat;

public sealed class OrToolsScheduleSolver
{
    public ScheduleResult Solve(ScheduleInput input)
    {
        try
        {
            var model = new CpModel();

            var sessions = BuildSessions(input);

            var candidateOptions = new List<CandidateOption>();

            foreach (var session in sessions)
            {
                foreach (var professor in input.Professors)
                {
                    if (!ProfessorCanTeachUnit(professor, session.Unit))
                        continue;

                    foreach (var timeSlot in input.TimeSlots)
                    {
                        if (!TimeSlotMatchesUnit(timeSlot, session.Unit))
                            continue;

                        if (timeSlot.DurationMinutes < session.Unit.SessionDurationMinutes)
                            continue;

                        if (!ProfessorIsAvailable(professor, timeSlot, session.Unit.UniversityId))
                            continue;

                        candidateOptions.Add(new CandidateOption
                        {
                            SessionId = session.Id,
                            ProfessorId = professor.Id,
                            TimeSlotId = timeSlot.Id
                        });
                    }
                }
            }

            var unscheduledSessions = new List<UnscheduledSessionDto>();
            var sessionById = sessions.ToDictionary(x => x.Id);

            // حذف sessionهای بدون هیچ گزینه
            var remainingSessions = new List<UnitSession>();
            foreach (var session in sessions)
            {
                var options = candidateOptions.Where(x => x.SessionId == session.Id).ToList();
                if (options.Count == 0)
                {
                    unscheduledSessions.Add(new UnscheduledSessionDto
                    {
                        UnitKey = session.Unit.UnitKey,
                        SessionIndex = session.SessionIndex,
                        Reason = BuildNoFeasibleVariableDiagnostic(input, session)
                    });
                }
                else
                {
                    remainingSessions.Add(session);
                }
            }

            // رفع forced collision ها قبل از ساخت مدل
            bool changed;
            do
            {
                changed = false;

                var optionsBySession = candidateOptions
                    .GroupBy(x => x.SessionId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var forcedOptions = remainingSessions
                    .Where(s => optionsBySession.ContainsKey(s.Id) && optionsBySession[s.Id].Count == 1)
                    .Select(s => new
                    {
                        Session = s,
                        Option = optionsBySession[s.Id][0]
                    })
                    .ToList();

                // conflict استاد-بازه
                var professorSlotConflicts = forcedOptions
                    .GroupBy(x => new { x.Option.ProfessorId, x.Option.TimeSlotId })
                    .Where(g => g.Count() > 1)
                    .ToList();

                foreach (var group in professorSlotConflicts)
                {
                    var losers = group.Skip(1).ToList();
                    foreach (var loser in losers)
                    {
                        remainingSessions.RemoveAll(x => x.Id == loser.Session.Id);
                        candidateOptions.RemoveAll(x => x.SessionId == loser.Session.Id);

                        unscheduledSessions.Add(new UnscheduledSessionDto
                        {
                            UnitKey = loser.Session.Unit.UnitKey,
                            SessionIndex = loser.Session.SessionIndex,
                            Reason = $"Forced conflict on Professor={group.Key.ProfessorId}, TimeSlot={group.Key.TimeSlotId}"
                        });

                        changed = true;
                    }
                }

                if (changed)
                    continue;

                // conflict واحد-بازه
                var unitSlotConflicts = forcedOptions
                    .GroupBy(x => new { x.Session.Unit.UnitKey, x.Option.TimeSlotId })
                    .Where(g => g.Count() > 1)
                    .ToList();

                foreach (var group in unitSlotConflicts)
                {
                    var losers = group.Skip(1).ToList();
                    foreach (var loser in losers)
                    {
                        remainingSessions.RemoveAll(x => x.Id == loser.Session.Id);
                        candidateOptions.RemoveAll(x => x.SessionId == loser.Session.Id);

                        unscheduledSessions.Add(new UnscheduledSessionDto
                        {
                            UnitKey = loser.Session.Unit.UnitKey,
                            SessionIndex = loser.Session.SessionIndex,
                            Reason = $"Forced conflict on Unit={group.Key.UnitKey}, TimeSlot={group.Key.TimeSlotId}"
                        });

                        changed = true;
                    }
                }

            } while (changed);

            if (remainingSessions.Count == 0)
            {
                return new ScheduleResult
                {
                    Status = unscheduledSessions.Count > 0
                        ? ScheduleSolveStatus.Partial
                        : ScheduleSolveStatus.Infeasible,
                    Message = "No schedulable sessions found.",
                    Assignments = [],
                    UnscheduledSessions = unscheduledSessions
                };
            }

            var variables = new Dictionary<(long SessionId, long ProfessorId, long TimeSlotId), BoolVar>();

            foreach (var option in candidateOptions)
            {
                var variableName = $"x_s{option.SessionId}_p{option.ProfessorId}_t{option.TimeSlotId}";
                variables[(option.SessionId, option.ProfessorId, option.TimeSlotId)] = model.NewBoolVar(variableName);
            }

            // هر session دقیقاً یک انتخاب
            foreach (var session in remainingSessions)
            {
                var sessionVars = variables
                    .Where(x => x.Key.SessionId == session.Id)
                    .Select(x => x.Value)
                    .ToList();

                if (sessionVars.Count == 0)
                {
                    unscheduledSessions.Add(new UnscheduledSessionDto
                    {
                        UnitKey = session.Unit.UnitKey,
                        SessionIndex = session.SessionIndex,
                        Reason = "Session lost all options during preprocessing."
                    });

                    continue;
                }

                model.Add(LinearExpr.Sum(sessionVars) <= 1);
            }

            // هر استاد در هر slot حداکثر یک جلسه
            foreach (var professor in input.Professors)
            {
                foreach (var timeSlot in input.TimeSlots)
                {
                    var vars = variables
                        .Where(x => x.Key.ProfessorId == professor.Id && x.Key.TimeSlotId == timeSlot.Id)
                        .Select(x => x.Value)
                        .ToList();

                    if (vars.Count > 1)
                    {
                        model.Add(LinearExpr.Sum(vars) <= 1);
                    }
                }
            }

            // هر unit در هر slot حداکثر یک session
            var remainingSessionById = remainingSessions.ToDictionary(x => x.Id);

            foreach (var unitKey in remainingSessions.Select(x => x.Unit.UnitKey).Distinct())
            {
                foreach (var timeSlot in input.TimeSlots)
                {
                    var vars = variables
                        .Where(x =>
                            x.Key.TimeSlotId == timeSlot.Id &&
                            remainingSessionById.TryGetValue(x.Key.SessionId, out var s) &&
                            s.Unit.UnitKey == unitKey)
                        .Select(x => x.Value)
                        .ToList();

                    if (vars.Count > 1)
                    {
                        model.Add(LinearExpr.Sum(vars) <= 1);
                    }
                }
            }
            model.Maximize(LinearExpr.Sum(variables.Values));
            var solver = new CpSolver();
            solver.StringParameters = "max_time_in_seconds:30 log_search_progress:true";

            var status = solver.Solve(model);
            Console.WriteLine(solver.ResponseStats());

            if (status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible)
            {
                return new ScheduleResult
                {
                    Status = unscheduledSessions.Count > 0
                        ? ScheduleSolveStatus.Partial
                        : ScheduleSolveStatus.Infeasible,
                    Message = "Solver could not find a feasible solution after preprocessing.",
                    Assignments = [],
                    UnscheduledSessions = unscheduledSessions
                };
            }

            var assignments = new List<ScheduleAssignmentDto>();

            foreach (var kvp in variables)
            {
                if (solver.Value(kvp.Value) != 1)
                    continue;

                var session = remainingSessions.First(s => s.Id == kvp.Key.SessionId);
                var professor = input.Professors.First(p => p.Id == kvp.Key.ProfessorId);
                var timeSlot = input.TimeSlots.First(t => t.Id == kvp.Key.TimeSlotId);

                foreach (var offering in session.Unit.Offerings)
                {
                    assignments.Add(new ScheduleAssignmentDto
                    {
                        CourseOfferingId = offering.CourseOfferingId,
                        ProfessorId = professor.Id,
                        TimeSlotId = timeSlot.Id,
                        RoomId = input.DefaultRoomId
                    });
                }
            }

            return new ScheduleResult
            {
                Status = unscheduledSessions.Count == 0
                    ? (status == CpSolverStatus.Optimal
                        ? ScheduleSolveStatus.Optimal
                        : ScheduleSolveStatus.Feasible)
                    : ScheduleSolveStatus.Partial,
                Message = unscheduledSessions.Count == 0
                    ? "Schedule generated successfully."
                    : "Schedule generated partially. Some sessions could not be scheduled.",
                Assignments = assignments,
                UnscheduledSessions = unscheduledSessions
            };
        }
        catch (Exception ex)
        {
            return new ScheduleResult
            {
                Status = ScheduleSolveStatus.Failed,
                Message = $"Solver failed: {ex.Message}",
                Assignments = [],
                UnscheduledSessions = []
            };
        }
    }

    private static List<UnitSession> BuildSessions(ScheduleInput input)
    {
        var result = new List<UnitSession>();
        long sessionId = 1;

        foreach (var unit in input.Units)
        {
            for (int i = 0; i < unit.WeeklySessionCount; i++)
            {
                result.Add(new UnitSession
                {
                    Id = sessionId++,
                    Unit = unit,
                    SessionIndex = i
                });
            }
        }

        return result;
    }

    private static bool ProfessorCanTeachUnit(ProfessorSolverDto professor, SchedulingUnit unit)
    {
        var professorCourseIds = professor.Skills
            .Select(x => x.CourseId)
            .ToHashSet();

        return unit.CourseIds.All(courseId => professorCourseIds.Contains(courseId));
    }

    private static bool TimeSlotMatchesUnit(TimeSlotSolverDto timeSlot, SchedulingUnit unit)
    {
        return timeSlot.UniversityId == unit.UniversityId;
    }

    private static bool ProfessorIsAvailable(
        ProfessorSolverDto professor,
        TimeSlotSolverDto timeSlot,
        long universityId)
    {
        return professor.Availabilities.Any(a =>
            a.UniversityId == universityId &&
            a.DayOfWeek == timeSlot.DayOfWeek &&
            a.StartMinutes <= timeSlot.StartMinutes &&
            a.EndMinutes >= timeSlot.EndMinutes);
    }

    private static string BuildNoFeasibleVariableDiagnostic(ScheduleInput input, UnitSession session)
    {
        var unit = session.Unit;

        var professorsWithSkill = input.Professors
            .Where(p => ProfessorCanTeachUnit(p, unit))
            .ToList();

        var matchingUniversityTimeSlots = input.TimeSlots
            .Where(t => TimeSlotMatchesUnit(t, unit))
            .ToList();

        var longEnoughTimeSlots = matchingUniversityTimeSlots
            .Where(t => t.DurationMinutes >= unit.SessionDurationMinutes)
            .ToList();

        var availableProfessorTimeSlotPairs = new List<(long ProfessorId, long TimeSlotId)>();

        foreach (var professor in professorsWithSkill)
        {
            foreach (var timeSlot in longEnoughTimeSlots)
            {
                if (ProfessorIsAvailable(professor, timeSlot, unit.UniversityId))
                {
                    availableProfessorTimeSlotPairs.Add((professor.Id, timeSlot.Id));
                }
            }
        }

        if (professorsWithSkill.Count == 0)
            return "No professor has required skills.";

        if (matchingUniversityTimeSlots.Count == 0)
            return "No matching university time slots found.";

        if (longEnoughTimeSlots.Count == 0)
            return "All matching time slots are shorter than required session duration.";

        if (availableProfessorTimeSlotPairs.Count == 0)
            return "No professor availability matches any valid time slot.";

        return "No feasible professor/time slot combination found.";
    }

    private sealed class CandidateOption
    {
        public long SessionId { get; set; }
        public long ProfessorId { get; set; }
        public long TimeSlotId { get; set; }
    }
}
