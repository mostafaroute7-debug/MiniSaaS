namespace MiniSaaS.Application.Common.Models;

public class ResultDto<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public IReadOnlyCollection<string>? Errors { get; init; }
    public static ResultDto<T> Ok(T data,string? message = null)
    {
        return new ResultDto<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ResultDto<T> Failure(string message,IReadOnlyCollection<string>? errors = null)
    {
        return new ResultDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}