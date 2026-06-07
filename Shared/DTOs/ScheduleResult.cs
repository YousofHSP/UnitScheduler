namespace Shared.DTOs;

public class ScheduleAssignment
{
    public long UnitId { get; set; }
    public long ProfessorId { get; set; }
    public long TimeSlotId { get; set; }
}
public class ScheduleResult
{
    public bool IsSuccess { get; private set; }

    public bool IsInfeasible { get; private set; }

    public string? ErrorMessage { get; private set; }

    public List<ScheduleAssignment> Assignments { get; private set; } = new();

    public static ScheduleResult Success(List<ScheduleAssignment> assignments)
    {
        return new ScheduleResult
        {
            IsSuccess = true,
            IsInfeasible = false,
            Assignments = assignments
        };
    }

    public static ScheduleResult Infeasible(string errorMessage)
    {
        return new ScheduleResult
        {
            IsSuccess = false,
            IsInfeasible = true,
            ErrorMessage = errorMessage
        };
    }

    public static ScheduleResult Failed(string errorMessage)
    {
        return new ScheduleResult
        {
            IsSuccess = false,
            IsInfeasible = false,
            ErrorMessage = errorMessage
        };
    }
}
