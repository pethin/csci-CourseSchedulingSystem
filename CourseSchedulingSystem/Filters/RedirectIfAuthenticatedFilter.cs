using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CourseSchedulingSystem.Filters
{
    public class RedirectIfAuthenticatedFilter : ResultFilterAttribute
    {
        public RedirectIfAuthenticatedFilter()
        {
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var urlHelper = new UrlHelper(context);

            if (context.HttpContext.User?.Identity.IsAuthenticated ?? false)
            {
                context.Result = new LocalRedirectResult(urlHelper.Content("~/Manage/"));
            }

            base.OnResultExecuting(context);
        }
    }
}
