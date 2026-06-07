namespace Service.Engine.Solver;
public sealed class ProfessorAvailabilitySolverDto
{
    public long UniversityId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int StartMinutes { get; set; }
    public int EndMinutes { get; set; }
}
