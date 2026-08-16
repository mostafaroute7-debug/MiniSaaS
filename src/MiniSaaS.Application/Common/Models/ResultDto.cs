namespace MiniSaaS.Application.Common.Models;

public class ResultDto<T>
{
    public bool Success { get; init; }

    public T? Data { get; init; }

    public string? Message { get; init; }

    public ErrorCode ErrorCode { get; init; }

    public IReadOnlyList<string>? Errors { get; init; }

    public static ResultDto<T> Ok(
        T data,
        string? message = null)
    {
        return new ResultDto<T>
        {
            Success = true,
            Data = data,
            Message = message,
            ErrorCode = ErrorCode.None
        };
    }

    public static ResultDto<T> Failure(
        string message,
        ErrorCode errorCode = ErrorCode.InternalServerError,
        IReadOnlyList<string>? errors = null)
    {
        return new ResultDto<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors
        };
    }
}