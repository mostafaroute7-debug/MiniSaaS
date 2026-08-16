using MiniSaaS.Application.Common.Models;

namespace MiniSaaS.Application.Common.Exceptions;

public sealed class BusinessException : Exception
{
    public ErrorCode ErrorCode { get; }
    public BusinessException(string message,ErrorCode errorCode): base(message)
    {
        ErrorCode = errorCode;
    }
}