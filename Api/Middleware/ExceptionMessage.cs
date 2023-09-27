namespace Api.Middleware;

public class ExceptionMessage
{
    public required int StatusCode { get; set; }
    public required string Message { get; set; } 
    public List<ErrorMessage> Errors { get; set; } = new();
}