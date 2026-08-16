using Microsoft.AspNetCore.Mvc;
using MiniSaaS.Application.Common.Models;

namespace MiniSaaS.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, ResultDto<T> result)
    {
        if (result.Success)
        {
            return controller.Ok(result);
        }

        return result.ErrorCode switch
        {
            ErrorCode.Validation => controller.BadRequest(result),

            ErrorCode.NotFound => controller.NotFound(result),

            ErrorCode.Conflict => controller.Conflict(result),

            ErrorCode.Unauthorized => controller.Unauthorized(result),

            ErrorCode.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden,result),

            ErrorCode.TenantRequired =>controller.BadRequest(result),

            _ =>controller.StatusCode(StatusCodes.Status500InternalServerError,result)
        };
    }
}