using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CourseSchedulingSystem.Filters
{
    /// <summary>
    /// Filter that redirects the user to "/Manage/" if the user is authenticated.
    /// </summary>
    public class RedirectIfAuthenticatedFilter : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var urlHelper = new UrlHelper(context);

            if (context.HttpContext.User?.Identity.IsAuthenticated ?? false)
                context.Result = new LocalRedirectResult(urlHelper.Content("~/Manage/"));

            base.OnResultExecuting(context);
        }
    }
}