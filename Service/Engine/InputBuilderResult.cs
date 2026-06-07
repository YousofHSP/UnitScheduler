using Service.Engine.Solver;

namespace Service.Engine;
public sealed class InputBuilderResult
{
    public bool IsSuccess { get; init; }
    public string ErrorMessage { get; init; } = "";
    public ScheduleInput? Input { get; init; }

    public static InputBuilderResult Success(ScheduleInput input)
    {
        return new InputBuilderResult
        {
            IsSuccess = true,
            Input = input
        };
    }

    public static InputBuilderResult Failed(string message)
    {
        return new InputBuilderResult
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }
}
