using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [ProtectedPersonalData]
        [Required]
        [Display(Name = "User Name")]
        public override string UserName { get; set; }

        [Display(Name = "Lockout Enabled")] public override bool LockoutEnabled { get; set; }

        public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }
    }
}