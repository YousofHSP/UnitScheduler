namespace SchedulingService.Implementation;

public class SolverConfiguration
{
    public string SearchStrategy { get; set; } = "HillClimbing";
    public double InitialTemperature { get; set; } = 100;
    public double CoolingRate { get; set; } = 0.995;
    public int MaxIterations { get; set; } = 50000;
    public Dictionary<string, double> ConstraintWeights { get; set; } = new();

    public static async Task<SolverConfiguration> LoadAsync(ISchedulingRepository repo, int termId)
    {
        // This would query a hypothetical SolverParameter table.
        // For now, return defaults.
        return new SolverConfiguration();
    }
}