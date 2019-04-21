using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>
    /// Represents a user in the identity system.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>Gets or sets the username for this user.</summary>
        [ProtectedPersonalData]
        [Required]
        [Display(Name = "User Name")]
        public override string UserName { get; set; }

        /// <summary>Gets or sets the lockout flag for this user.</summary>
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

        /// <summary>Navigation property for the departments associations this user belongs to.</summary>
        public List<DepartmentUser> DepartmentUsers { get; set; }
    }
}