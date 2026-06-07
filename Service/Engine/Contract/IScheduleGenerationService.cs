using Shared.DTOs.Engine;

namespace Service.Engine.Contract;
public interface IScheduleGenerationService
{
    Task<ScheduleResult> GenerateAsync(
        ScheduleSolveRequest request,
        CancellationToken ct = default);
}
