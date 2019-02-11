using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        [NotMapped]
        public static class RoleNames
        {
            public static string Administrator = "Administrator";
            public static string AssociateDean = "Associate Dean";
            public static string DepartmentChair = "Department Chair";
        }

        public string Description { get; set; }
    }
}