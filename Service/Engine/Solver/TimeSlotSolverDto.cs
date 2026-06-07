namespace Service.Engine.Solver;
public sealed class TimeSlotSolverDto
{
    public long Id { get; set; }
    public long UniversityId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int StartMinutes { get; set; }
    public int EndMinutes { get; set; }

    public int DurationMinutes => EndMinutes - StartMinutes;
}
