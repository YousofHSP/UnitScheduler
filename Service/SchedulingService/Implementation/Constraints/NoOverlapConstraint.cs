using SchedulingService.Contracts;

namespace SchedulingService.Implementation.Constraints;

public class NoOverlapConstraint : IConstraint
{
    public double Evaluate(AssignmentSolution solution, SchedulingContext context)
    {
        var profTime = new Dictionary<int, HashSet<int>>();
        var roomTime = new Dictionary<int, HashSet<int>>();
        var cohortTime = new Dictionary<string, HashSet<int>>();

        foreach (var kv in solution.Assignments)
        {
            int classId = kv.Key;
            var (profId, roomId, timeSlotId) = kv.Value;
            var cls = context.Classes.First(c => c.Id == classId);

            // Professor
            if (!profTime.ContainsKey(profId))
                profTime[profId] = new HashSet<int>();
            if (!profTime[profId].Add(timeSlotId))
                return double.MaxValue;

            // Room
            if (!roomTime.ContainsKey(roomId))
                roomTime[roomId] = new HashSet<int>();
            if (!roomTime[roomId].Add(timeSlotId))
                return double.MaxValue;

            // Cohort
            if (!cohortTime.ContainsKey(cls.CohortKey))
                cohortTime[cls.CohortKey] = new HashSet<int>();
            if (!cohortTime[cls.CohortKey].Add(timeSlotId))
                return double.MaxValue;
        }
        return 0;
    }
}