namespace Service.Engine.Contract;

public interface IEngineService
{
    Task FinalProcess(CancellationToken ct);
}