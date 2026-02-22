using SchedulingService.Contracts;

namespace SchedulingService.Implementation;

public class ConstraintEvaluator
{
    private readonly List<(IConstraint constraint, double weight)> _constraints;

    public ConstraintEvaluator(List<(IConstraint, double)> constraints)
    {
        _constraints = constraints;
    }

    public double Evaluate(AssignmentSolution solution, SchedulingContext context)
    {
        double total = 0;
        foreach (var (constraint, weight) in _constraints)
        {
            var val = constraint.Evaluate(solution, context);
            if (val >= double.MaxValue / 2)
                return double.MaxValue;
            total += val * weight;
        }
        return total;
    }
}