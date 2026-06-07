namespace Shared.DTOs.Engine;
public sealed class ScheduleResult
{
    public ScheduleSolveStatus Status { get; init; }
    public string Message { get; init; } = "";
    public List<ScheduleAssignmentDto> Assignments { get; init; } = [];
    public List<UnscheduledSessionDto> UnscheduledSessions { get; init; } = [];
    public double? ObjectiveValue { get; init; }
    public double? WallTimeSeconds { get; init; }
    public long? ConflictCount { get; init; }
    public long? BranchCount { get; init; }

    public bool IsSuccess =>
        Status == ScheduleSolveStatus.Feasible ||
        Status == ScheduleSolveStatus.Optimal;

    public static ScheduleResult Optimal(
        List<ScheduleAssignmentDto> assignments,
        double objective,
        double wallTime,
        long conflicts,
        long branches)
    {
        return new ScheduleResult
        {
            Status = ScheduleSolveStatus.Optimal,
            Message = "Optimal schedule found.",
            Assignments = assignments,
            ObjectiveValue = objective,
            WallTimeSeconds = wallTime,
            ConflictCount = conflicts,
            BranchCount = branches
        };
    }

    public static ScheduleResult Feasible(
        List<ScheduleAssignmentDto> assignments,
        double objective,
        double wallTime,
        long conflicts,
        long branches)
    {
        return new ScheduleResult
        {
            Status = ScheduleSolveStatus.Feasible,
            Message = "Feasible schedule found.",
            Assignments = assignments,
            ObjectiveValue = objective,
            WallTimeSeconds = wallTime,
            ConflictCount = conflicts,
            BranchCount = branches
        };
    }

    public static ScheduleResult Infeasible(string message)
    {
        return new ScheduleResult
        {
            Status = ScheduleSolveStatus.Infeasible,
            Message = message
        };
    }

    public static ScheduleResult Failed(string message)
    {
        return new ScheduleResult
        {
            Status = ScheduleSolveStatus.Failed,
            Message = message
        };
    }
}
