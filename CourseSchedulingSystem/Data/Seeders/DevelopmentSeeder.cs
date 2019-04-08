using System.Collections.Generic;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class DevelopmentSeeder : ISeeder
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public DevelopmentSeeder(
            IHostingEnvironment env,
            ILogger<DevelopmentSeeder> logger,
            UserManager<ApplicationUser> userManager)
        {
            _env = env;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (!_env.IsDevelopment())
            {
                _logger.LogWarning("Skipping... This seeder can only be run in development env.");
                return;
            }

            await Task.Yield();
            
            // Not needed since using ADFS
//            await SeedDevelopmentUsers();
        }

        /// <summary>
        ///     Creates a user with the specified user name if it does not exist and adds the user
        ///     to the specified role.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <param name="password">Default password is `Password123!`</param>
        /// <returns></returns>
        private async Task<IdentityResult> CreateUserWithRoleAsync(
            string userName,
            string roleName,
            string password = "Password123!")
        {
            IdentityResult result;
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                user = new ApplicationUser {UserName = userName};
                result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded) return result;
            }

            if (roleName != null && !await _userManager.IsInRoleAsync(user, roleName))
            {
                result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded) return result;
            }

            return IdentityResult.Success;
        }

        /// <summary>
        ///     Seeds one of each type of user: admin, associate dean, department chair, and role-less.
        /// </summary>
        /// <returns></returns>
        private async Task SeedDevelopmentUsers()
        {
            // Create an admin user.
            var result = await CreateUserWithRoleAsync("admin", ApplicationRole.RoleNames.Administrator);
            if (!result.Succeeded) LogIdentityResultErrors(result.Errors);

            // Create an associate dean.
            result = await CreateUserWithRoleAsync("dean", ApplicationRole.RoleNames.AssociateDean);
            if (!result.Succeeded) LogIdentityResultErrors(result.Errors);

            // Create a department chair.
            result = await CreateUserWithRoleAsync("chair", ApplicationRole.RoleNames.DepartmentChair);
            if (!result.Succeeded) LogIdentityResultErrors(result.Errors);

            // Create a role-less user.
            result = await CreateUserWithRoleAsync("norole", null);
            if (!result.Succeeded) LogIdentityResultErrors(result.Errors);
        }

        /// <summary>
        ///     Logs identity errors to error level.
        /// </summary>
        /// <param name="errors"></param>
        private void LogIdentityResultErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors) _logger.LogError(error.Description, error);
        }
    }
}