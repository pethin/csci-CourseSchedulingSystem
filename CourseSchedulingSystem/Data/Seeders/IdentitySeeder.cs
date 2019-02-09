using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class IdentitySeeder : ISeeder
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public IdentitySeeder(
            IHostingEnvironment env,
            ILogger<IdentitySeeder> logger,
            IServiceProvider serviceProvider)
        {
            _env = env;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task SeedAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                await SeedRolesAsync(roleManager);

                if (_env.IsDevelopment())
                {
                    await SeedDevelopmentUsers(userManager);
                }
            }
        }

        /// <summary>
        /// Create default roles if they do not exist.
        /// </summary>
        /// <param name="roleManager"></param>
        /// <returns></returns>
        private async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            IEnumerable<ApplicationRole> defaultRoles = typeof(ApplicationRole.Roles)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => (ApplicationRole) f.GetValue(null));

            var rolesNotYetCreated = defaultRoles.Where(defaultRole =>
                !roleManager.Roles.Any(dbRole => dbRole.Name == defaultRole.Name));

            // Create roles that have not been created
            foreach (var applicationRole in rolesNotYetCreated)
            {
                await roleManager.CreateAsync(applicationRole);
            }
        }

        /// <summary>
        /// Creates a user with the specified user name and adds the user
        /// to the specified role.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private async Task<IdentityResult> CreateUserWithRoleAsync(
            UserManager<ApplicationUser> userManager,
            string userName,
            string roleName)
        {
            var user = new ApplicationUser {UserName = userName};

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) return result;

            result = await userManager.AddToRoleAsync(user, roleName);
            return result;
        }

        /// <summary>
        /// Seeds one of each type of user: admin, associate dean, department chair, and role-less.
        /// </summary>
        /// <param name="userManager"></param>
        /// <returns></returns>
        private async Task SeedDevelopmentUsers(UserManager<ApplicationUser> userManager)
        {
            // If users already exist, then the database has already been seeded.
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            // Create an admin user.
            var result = await CreateUserWithRoleAsync(userManager, "admin", ApplicationRole.Roles.Administrator.Name);
            if (!result.Succeeded) LogIdentityResultErrors(result.Errors);

            // Create an associate dean.
            result = await CreateUserWithRoleAsync(userManager, "dean", ApplicationRole.Roles.AssociateDean.Name);
            if (!result.Succeeded) LogIdentityResultErrors(result.Errors);

            // Create a department chair.
            result = await CreateUserWithRoleAsync(userManager, "chair", ApplicationRole.Roles.DepartmentChair.Name);
            if (!result.Succeeded) LogIdentityResultErrors(result.Errors);

            // Create a role-less user.
            result = await userManager.CreateAsync(new ApplicationUser {UserName = "norole"});
            if (!result.Succeeded) LogIdentityResultErrors(result.Errors);
        }

        /// <summary>
        /// Logs identity errors to error level.
        /// </summary>
        /// <param name="errors"></param>
        private void LogIdentityResultErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                _logger.LogError(error.Description, error);
            }
        }
    }
}