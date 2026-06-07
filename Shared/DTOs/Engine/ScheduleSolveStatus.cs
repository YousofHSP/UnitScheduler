namespace Shared.DTOs.Engine;

public enum ScheduleSolveStatus
{
    Unknown = 0,
    Feasible = 1,
    Optimal = 2,
    Infeasible = 3,
    Failed = 4,
    Partial = 5
}