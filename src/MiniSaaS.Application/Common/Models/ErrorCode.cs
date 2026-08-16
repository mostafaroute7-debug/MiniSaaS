namespace MiniSaaS.Application.Common.Models;

public enum ErrorCode
{
    None = 0,
    Validation = 1000,
    NotFound = 1001,
    Conflict = 1002,
    Unauthorized = 1003,
    Forbidden = 1004,
    TenantRequired = 1005,
    InternalServerError = 1500
}