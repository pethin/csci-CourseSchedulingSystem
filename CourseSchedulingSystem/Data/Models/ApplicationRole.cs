using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>
    /// Represents a role in the identity system.
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
        /// <summary>Gets or sets the description for this role.</summary>
        public string Description { get; set; }

        [NotMapped]
        public static class RoleNames
        {
            public static string Administrator = "Administrator";
            public static string AssociateDean = "Associate Dean";
            public static string DepartmentChair = "Department Chair";
        }
    }
}