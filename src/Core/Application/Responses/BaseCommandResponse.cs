using System.Net;

namespace Application.Responses;

public class BaseCommandResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; }
    public List<string> Errors { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}

public class BaseCommandResponse<T> : BaseCommandResponse
{
    public T Data { get; set; }
}