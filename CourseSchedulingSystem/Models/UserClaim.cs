using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Models
{
    public class UserClaim : IdentityUserClaim<Guid>
    {
        public override Guid UserId { get; set; }
        public User User { get; set; }
    }
}
