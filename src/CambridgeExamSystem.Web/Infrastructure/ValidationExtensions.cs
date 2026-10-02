using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CambridgeExamSystem.Web.Infrastructure;

public static class ValidationExtensions
{
    public static async Task<bool> ValidateIntoAsync<T>(this IValidator<T> validator, T model, ModelStateDictionary modelState, string prefix = "")
    {
        var result = await validator.ValidateAsync(model);
        foreach (var error in result.Errors)
        {
            var key = string.IsNullOrEmpty(prefix) ? error.PropertyName : $"{prefix}.{error.PropertyName}";
            modelState.AddModelError(key, error.ErrorMessage);
        }

        return result.IsValid;
    }

    public static void AddErrors(this ModelStateDictionary modelState, IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            modelState.AddModelError(string.Empty, error);
        }
    }
}
