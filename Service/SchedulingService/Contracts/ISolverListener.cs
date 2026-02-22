namespace SchedulingService.Contracts;

public interface ISolverListener
{
    void OnProgress(int iteration, double currentPenalty, double bestPenalty);
    void OnComplete(AssignmentSolution finalSolution);
}