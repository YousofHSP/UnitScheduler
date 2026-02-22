using SchedulingService.Contracts;

namespace SchedulingService.Implementation.WeightProviders;

public class DefaultClassWeightProvider : IClassWeightProvider
{
    public double GetWeight(SchedulingClass cls)
    {
        // Weight based on student count and duration (more students/longer = higher priority)
        return cls.StudentCount * cls.DurationMinutes / 60.0;
    }
}