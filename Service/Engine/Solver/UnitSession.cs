namespace Service.Engine.Solver;
internal sealed class UnitSession
{
    public long Id { get; set; }
    public SchedulingUnit Unit { get; set; } = null!;
    public int SessionIndex { get; set; }
}
