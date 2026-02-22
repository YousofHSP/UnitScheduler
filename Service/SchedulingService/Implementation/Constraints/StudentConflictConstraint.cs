using SchedulingService.Contracts;

namespace SchedulingService.Implementation.Constraints;

public class StudentConflictConstraint : IConstraint
{
    public double Evaluate(AssignmentSolution solution, SchedulingContext context)
    {
        if (context.StudentEnrollments == null || !context.StudentEnrollments.Any())
            return 0;

        var studentTimeSlots = new Dictionary<int, HashSet<int>>();

        foreach (var kv in solution.Assignments)
        {
            int classId = kv.Key;
            var (_, _, timeSlotId) = kv.Value;

            // Find all students enrolled in this class
            foreach (var studentKv in context.StudentEnrollments)
            {
                int studentId = studentKv.Key;
                var enrolledClassIds = studentKv.Value;
                if (enrolledClassIds.Contains(classId))
                {
                    if (!studentTimeSlots.ContainsKey(studentId))
                        studentTimeSlots[studentId] = new HashSet<int>();
                    if (!studentTimeSlots[studentId].Add(timeSlotId))
                        return double.MaxValue; // student has two classes at same time
                }
            }
        }
        return 0;
    }
}