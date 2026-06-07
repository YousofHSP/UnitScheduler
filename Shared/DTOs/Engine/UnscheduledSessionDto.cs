namespace Shared.DTOs.Engine;
public sealed class UnscheduledSessionDto
{
    public string UnitKey { get; init; } = "";
    public int SessionIndex { get; init; }
    public string Reason { get; init; } = "";
}

