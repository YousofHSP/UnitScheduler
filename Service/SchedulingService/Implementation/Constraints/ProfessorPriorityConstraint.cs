using SchedulingService.Contracts;

namespace SchedulingService.Implementation.Constraints;

public class ProfessorPriorityConstraint : IConstraint
{
    public double Evaluate(AssignmentSolution solution, SchedulingContext context)
    {
        double penalty = 0;
        foreach (var kv in solution.Assignments)
        {
            int classId = kv.Key;
            var (profId, _, _) = kv.Value;
            var cls = context.Classes.First(c => c.Id == classId);

            if (context.SkillsByCourseId.TryGetValue(cls.CourseId, out var skills))
            {
                var skill = skills.FirstOrDefault(s => s.ProfessorId == profId);
                if (skill != null)
                {
                    penalty += skill.Priority * 10; // lower priority = lower penalty
                }
            }
        }
        return penalty;
    }
}