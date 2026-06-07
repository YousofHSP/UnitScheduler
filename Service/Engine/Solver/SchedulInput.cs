namespace Service.Engine.Solver;
public sealed class ScheduleInput
{
    public List<SchedulingUnit> Units { get; set; } = [];
    public List<ProfessorSolverDto> Professors { get; set; } = [];
    public List<TimeSlotSolverDto> TimeSlots { get; set; } = [];

    /// <summary>
    /// CourseId -> all prerequisite CourseIds, direct and indirect.
    /// </summary>
    public Dictionary<long, HashSet<long>> PrerequisiteClosure { get; set; } = [];

    public long DefaultRoomId { get; set; }
    public int TimeLimitSeconds { get; set; }
}
