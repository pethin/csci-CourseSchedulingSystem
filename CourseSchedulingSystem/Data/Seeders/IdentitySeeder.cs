using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class IdentitySeeder : ISeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentitySeeder(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// Template for creating roles.
        /// </summary>
        private static readonly List<ApplicationRole> RoleTemplates = new List<ApplicationRole>
        {
            new ApplicationRole
            {
                Id = new Guid(),
                Name = ApplicationRole.RoleNames.Administrator,
                Description = "Grants all permissions."
            },
            new ApplicationRole
            {
                Id = new Guid(),
                Name = ApplicationRole.RoleNames.AssociateDean,
                Description = "An associate dean."
            },
            new ApplicationRole
            {
                Id = new Guid(),
                Name = ApplicationRole.RoleNames.DepartmentChair,
                Description = "A department chair."
            }
        };

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
        }

        /// <summary>
        /// Create default roles if they do not exist.
        /// </summary>
        /// <returns></returns>
        private async Task SeedRolesAsync()
        {
            // Create roles that have not been created
            foreach (var role in RoleTemplates)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    await _roleManager.CreateAsync(role);
                }
            }
        }
    }
}