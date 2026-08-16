using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniSaaS.Application.Common.Models;

namespace MiniSaaS.API.Filters;

public sealed class ValidationFilter<T> : IAsyncActionFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue("request",out var value) || value is not T request)
        {
            await next();
            return;
        }

        var result = await _validator.ValidateAsync(request,context.HttpContext.RequestAborted);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();

            var response = ResultDto<object>.Failure( "One or more validation errors occurred.",ErrorCode.Validation,errors);

            context.Result = new BadRequestObjectResult(response);

            return;
        }

        await next();
    }
}