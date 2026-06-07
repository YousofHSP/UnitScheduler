namespace Shared.DTOs.Engine;

public class ScheduleSolveRequest
{
    public long TermId { get; set; }

    public long? UniversityId { get; set; } = 1;

    public long DefaultRoomId { get; set; } = 1;

    public bool ClearPreviousAssignments { get; set; } = true;

    public int TimeLimitSeconds { get; set; } = 60;
}