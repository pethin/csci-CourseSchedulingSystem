using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [ProtectedPersonalData]
        [Required]
        [Display(Name = "User Name")]
        public override string UserName { get; set; }

        [NotMapped]
        [Display(Name = "Locked Out")]
        public bool IsLockedOut
        {
            get => LockoutEnabled && LockoutEnd != null && LockoutEnd > DateTime.Now;
            set
            {
                if (value)
                {
                    LockoutEnabled = true;
                    LockoutEnd = DateTime.MaxValue.AddYears(-1);
                }
                else
                {
                    LockoutEnd = null;
                }
            }
        }

        public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }
    }
}