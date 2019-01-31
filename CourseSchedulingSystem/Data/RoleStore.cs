using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data
{
    public class RoleStore : RoleStore<Role, ApplicationDbContext, Guid, UserRole, RoleClaim>
    {
        public RoleStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }
    }
}
