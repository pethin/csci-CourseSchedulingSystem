using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Models
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public override Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
