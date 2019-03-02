using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [ProtectedPersonalData]
        [Required]
        [Display(Name = "User Name")]
        public override string UserName { get; set; }

        [Display(Name = "Lockout Enabled")]
        public override bool LockoutEnabled { get; set; }

        public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }
    }
}
