using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Seeds
{
    public static class IdentitySeeder
    {
        public static async Task RunAsync(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
        }

        /// <summary>
        /// Create default roles if they do not exist.
        /// </summary>
        /// <param name="roleManager"></param>
        /// <returns></returns>
        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            List<ApplicationRole> roles = typeof(ApplicationRole.Roles)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => (ApplicationRole) f.GetValue(null))
                .ToList();

            foreach (var applicationRole in roles)
            {
                if (await roleManager.FindByNameAsync(applicationRole.Name) == null)
                {
                    await roleManager.CreateAsync(applicationRole);
                }
            }
        }
    }
}