namespace Service.Engine.Solver;
public sealed class SchedulingUnit
{
    public string UnitKey { get; set; } = "";

    /// <summary>
    /// اگر تلفیقی باشد، چند Offering دارد.
    /// اگر عادی باشد، فقط یک Offering دارد.
    /// </summary>
    public List<UnitOffering> Offerings { get; set; } = [];

    public int WeeklySessionCount { get; set; }
    public int SessionDurationMinutes { get; set; }

    public HashSet<long> CourseIds { get; set; } = [];
    public long UniversityId { get; set; }
}
