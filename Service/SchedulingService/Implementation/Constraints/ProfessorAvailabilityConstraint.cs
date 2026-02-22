using SchedulingService.Contracts;

namespace SchedulingService.Implementation.Constraints;

public class ProfessorAvailabilityConstraint : IConstraint
{
    public double Evaluate(AssignmentSolution solution, SchedulingContext context)
    {
        foreach (var kv in solution.Assignments)
        {
            int classId = kv.Key;
            var (profId, _, timeSlotId) = kv.Value;
            var cls = context.Classes.First(c => c.Id == classId);
            var timeSlot = context.TimeSlotById[timeSlotId];

            if (!context.AvailabilityByProfessorId.TryGetValue(profId, out var byDay) ||
                !byDay.TryGetValue(timeSlot.DayOfWeek, out var slots))
            {
                return double.MaxValue;
            }

            bool available = slots.Any(s => s.StartTime.TotalMinutes <= timeSlot.StartMinute &&
                                             s.EndTime.TotalMinutes >= timeSlot.EndMinute);
            if (!available)
                return double.MaxValue;
        }
        return 0;
    }
}