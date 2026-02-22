using SchedulingService.Contracts;

namespace SchedulingService.Implementation.Constraints;

public class RoomCapacityConstraint : IConstraint
{
    public double Evaluate(AssignmentSolution solution, SchedulingContext context)
    {
        double penalty = 0;
        foreach (var kv in solution.Assignments)
        {
            int classId = kv.Key;
            var (_, roomId, _) = kv.Value;
            var cls = context.Classes.First(c => c.Id == classId);
            var room = context.RoomById[roomId];

            if (room.Capacity < cls.StudentCount)
                penalty += (cls.StudentCount - room.Capacity) * 5;
        }
        return penalty;
    }
}