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
        public static class Roles
        {
            public static ApplicationRole Administrator = new ApplicationRole
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = "Administrator",
                Description = "Grants all permissions."
            };
            public static ApplicationRole AssociateDean = new ApplicationRole
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                Name = "Associate Dean",
                Description = "An associate dean."
            };
            public static ApplicationRole DepartmentChair = new ApplicationRole
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
                Name = "Department Chair",
                Description = "A department chair."
            };
        }

        public string Description { get; set; }
    }
}
