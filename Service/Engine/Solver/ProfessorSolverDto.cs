namespace Service.Engine.Solver;
public sealed class ProfessorSolverDto
{
    public long Id { get; set; }
    public int MaxWeeklyMinutes { get; set; }

    public List<ProfessorSkillSolverDto> Skills { get; set; } = [];
    public List<ProfessorAvailabilitySolverDto> Availabilities { get; set; } = [];
}
