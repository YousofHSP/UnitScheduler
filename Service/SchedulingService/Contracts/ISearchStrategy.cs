using SchedulingService;

namespace SchedulingService.Contracts;

public interface ISearchStrategy
{
    void Initialize(AssignmentSolution solution, SchedulingContext context);
    bool Iterate();
    AssignmentSolution BestSolution { get; }
}