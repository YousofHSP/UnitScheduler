using SchedulingService.Contracts;

namespace SchedulingService.Implementation.Constraints;

public class ProfessorSkillConstraint : IConstraint
{
    public double Evaluate(AssignmentSolution solution, SchedulingContext context)
    {
        foreach (var kv in solution.Assignments)
        {
            int classId = kv.Key;
            var (profId, _, _) = kv.Value;
            var cls = context.Classes.First(c => c.Id == classId);

            if (!context.SkillsByCourseId.TryGetValue(cls.CourseId, out var skills) ||
                !skills.Any(s => s.ProfessorId == profId))
            {
                return double.MaxValue;
            }
        }
        return 0;
    }
}