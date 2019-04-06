using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Utilities
{
    public static class IIdentityExtensions
    {
        public static string UserName(this IIdentity identity)
        {
            var userEmail = identity.Name;
            return userEmail.Split("@").FirstOrDefault();
        }
    }
}
