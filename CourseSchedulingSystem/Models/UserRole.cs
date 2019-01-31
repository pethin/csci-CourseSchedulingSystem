using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Models
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public override Guid UserId { get; set; }
        public User User { get; set; }

        public override Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
