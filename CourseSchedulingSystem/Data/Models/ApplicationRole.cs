using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Data.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
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