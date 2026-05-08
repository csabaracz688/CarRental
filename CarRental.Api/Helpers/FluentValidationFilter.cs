using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRental.WebApi.Helpers;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var validationResult = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {
                context.Result = new BadRequestObjectResult(validationResult.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                }));

                return;
            }
        }

        await next();
    }
}