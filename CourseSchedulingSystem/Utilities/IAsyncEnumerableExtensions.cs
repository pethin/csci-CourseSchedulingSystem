using System.Collections.Async;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CourseSchedulingSystem.Utilities
{
    public static class IAsyncEnumerableExtensions
    {
        public static async Task AddErrorsToModelState(this IAsyncEnumerable<ValidationResult> asyncEnumerable, ModelStateDictionary modelState)
        {
            await asyncEnumerable.ForEachAsync(result =>
            {
                modelState.AddModelError(string.Empty, result.ErrorMessage);
            });
        }
    }
}
