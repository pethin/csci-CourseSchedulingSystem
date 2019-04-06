using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Filters
{
    public class UserLockoutFilter : IAsyncAuthorizationFilter
    {
        private readonly ApplicationDbContext _dbContext;

        public UserLockoutFilter(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
            {
                return;
            }

            var userName = context.HttpContext.User.Identity.UserName();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpper());

            if (user != null && user.IsLockedOut)
            {
                context.Result = new RedirectToPageResult("/Errors/LockedOut");
            }
        }

        private static bool SkipAuthorization(AuthorizationFilterContext context)
        {
            return context.ActionDescriptor.FilterDescriptors.Any(fd =>
                fd.Filter.GetType() == typeof(AllowAnonymousFilter));
        }
    }
}