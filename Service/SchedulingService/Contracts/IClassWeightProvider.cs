using SchedulingService;

namespace SchedulingService.Contracts;

public interface IClassWeightProvider
{
    double GetWeight(SchedulingClass cls);
}