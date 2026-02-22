using Common;

namespace Shared;

public class ApiResultClient
{
    public bool IsSuccess { get; set; }
    public ApiResultStatusCode StatusCode { get; set; }
    public string Message { get; set; }
    
}

public class ApiResultClient<T> : ApiResultClient 
{
    public T Data { get; set; }
    
}