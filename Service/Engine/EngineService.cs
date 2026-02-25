using System.Runtime.CompilerServices;
using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Engine.Contract;

namespace Service.Engine;

public class EngineService (
    IRepository<CourseOffering> courseOfferingRepository,
    IRepository<Professor> professorRepository,
    IRepository<Room> roomRepository,
    IRepository<TimeSlot> timeSlotRepository
): IEngineService
{
    private List<CourseOffering> courseOfferings;
    private List<Professor> professors;
    private List<Room> rooms;
    private List<TimeSlot> timeSlots;
    private List<AssignmentCandidate> assignmentCandidates;

    private async Task LoadData(CancellationToken ct)
    {
        courseOfferings = await courseOfferingRepository.TableNoTracking.ToListAsync(ct);
        professors = await professorRepository.TableNoTracking
            .Include(i => i.Skills)
            .Include(i => i.Presences)
            .Include(i => i.Availabilities)
            .ToListAsync(ct);
        rooms = await roomRepository.TableNoTracking.ToListAsync(ct);
        timeSlots = await timeSlotRepository.TableNoTracking.ToListAsync(ct);
    }

    public async Task FinalProcess(CancellationToken ct)
    {
        await Process(ct);
        var finalAssignment = new List<AssignmentCandidate>();
        var professorLoad = new Dictionary<long, int>();
        foreach (var offering in courseOfferings)
        {
            var canidates = assignmentCandidates
                .Where(i => i.CourseOffering.Id == offering.Id)
                .Where(c => HasPresence(c.Professor, offering))
                .Where(i => IsAvailable(i.Professor, i.TimeSlot))
                .Where(i => RoomFits(i.Room, offering))
                .Where(i => WithinWeeklyLoad(i.Professor, i.TimeSlot, professorLoad))
                .Where(i => !HasConflict(i, finalAssignment))
                .ToList();
            foreach (var c in canidates)
                c.Score = CalculateScore(c);
            var best = canidates
                .OrderByDescending(c => c.Score)
                .FirstOrDefault();
            if (best != null)
            {
                finalAssignment.Add(best);
                var duration = best.TimeSlot.EndMinute - best.TimeSlot.StartMinute;
                professorLoad[best.Professor.Id] =
                    professorLoad.GetValueOrDefault(best.Professor.Id) + duration;
            }
        }
    }

    private async Task Process(CancellationToken ct)
    {
        await LoadData(ct);

        foreach (var offering in courseOfferings)
        {
            var possibleProfessors = professors
                .Where(i => i.Skills.Any(s => s.CourseId == offering.CourseId)).ToList();

            foreach (var prof in possibleProfessors)
            {
                foreach (var slot in timeSlots.Where(i => i.UniversityId == offering.UniversityId))
                {
                    foreach (var room in rooms.Where(i => i.UniversityId == offering.UniversityId))
                    {
                        assignmentCandidates.Add(new AssignmentCandidate()
                        {
                            CourseOffering = offering,
                            Professor = prof,
                            TimeSlot = slot,
                            Room = room
                        });
                    }
                }
            }
        }
    }

    private bool HasPresence(Professor p, CourseOffering c)
    {
        return p.Presences.Any(i => i.UniversityId == c.UniversityId);
    }

    // دسترسی زمانی استاد
    private bool IsAvailable(Professor p, TimeSlot slot)
    {
        return p.Availabilities.Any(i =>
            i.UniversityId == slot.UniversityId &&
            i.DayOfWeek == slot.DayOfWeek &&
            slot.StartMinute >= i.StartMinutes &&
            slot.EndMinute <= i.EndMinutes);
    }

    // ظرفیت کلاس
    private bool RoomFits(Room r, CourseOffering c)
    {
        return r.Capacity >= c.StudentCount;
    }

    // بار کاری استاد
    private bool WithinWeeklyLoad(Professor p, TimeSlot slot, Dictionary<long, int> currentLoad)
    {
        var duration = slot.EndMinute - slot.StartMinute;
        return currentLoad.GetValueOrDefault(p.Id) + duration <= p.MaxWeeklyMinutes;
    }

    // عدم تداخل (استاد/کلاس)
    private bool HasConflict(AssignmentCandidate candidate, List<AssignmentCandidate> accepted)
    {
        return accepted.Any(a =>
            a.TimeSlot.DayOfWeek == candidate.TimeSlot.DayOfWeek &&
            !(candidate.TimeSlot.EndMinute <= a.TimeSlot.StartMinute ||
              candidate.TimeSlot.StartMinute >= a.TimeSlot.EndMinute) &&
            (a.Professor.Id == candidate.Professor.Id ||
             a.Room.Id == candidate.Room.Id)
        );
    }

    private int CalculateScore(AssignmentCandidate candidate)
    {
        var score = 0;
        var skill = professors
            .Where(i => i.Id == candidate.Professor.Id)
            .SelectMany(i => i.Skills)
            .Single(s => s.CourseId == candidate.CourseOffering.CourseId);
        score += skill.Priority switch
        {
            1 => 100,
            2 => 70,
            3 => 40,
            _ => 0
        };

        score += skill.SkillLevel * 10;
        if (candidate.Professor.HomeUniversityId == candidate.CourseOffering.UniversityId)
            score += 20;
        if (candidate.TimeSlot.StartMinute < 720)
            score += 15;
        return score;
    }
}