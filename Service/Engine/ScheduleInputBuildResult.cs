using Service.Engine.Solver;

namespace Service.Engine;
internal sealed class ScheduleInputBuildResult
{
    public bool IsSuccess { get; init; }
    public string ErrorMessage { get; init; } = "";
    public ScheduleInput? Input { get; init; }

    public static ScheduleInputBuildResult Success(ScheduleInput input)
    {
        return new ScheduleInputBuildResult
        {
            IsSuccess = true,
            Input = input
        };
    }

    public static ScheduleInputBuildResult Failed(string message)
    {
        return new ScheduleInputBuildResult
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }
}
