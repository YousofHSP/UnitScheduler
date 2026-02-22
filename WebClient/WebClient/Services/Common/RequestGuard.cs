namespace WebClient.Services.Common;

public class RequestGuard
{
    public bool IsAuthInvalid { get; private set; } = false;

    public void InvalidateAuth() => IsAuthInvalid = false;
    public void Reset() => IsAuthInvalid = false;
}