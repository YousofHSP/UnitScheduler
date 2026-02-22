using SchedulingService;

namespace SchedulingService.Contracts;

public interface ISolver
{
    Task<AssignmentSolution> SolveAsync(int termId, CancellationToken cancellationToken = default);
}