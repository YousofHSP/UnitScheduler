using SchedulingService;

namespace SchedulingService.Contracts;

public interface IConstraint
{
    double Evaluate(AssignmentSolution solution, SchedulingContext context);
}