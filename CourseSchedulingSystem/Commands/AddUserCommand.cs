using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Commands
{
    [Command("adduser", Description =
        "Add a user with no password. If the user exists, the user lockout will be removed.")]
    [HelpOption("--help")]
    public class AddUserCommand
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public AddUserCommand(IServiceProvider serviceProvider, ILogger<AddUserCommand> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [Argument(0, Name = "username")]
        [Required]
        public string Username { get; }

        public async Task OnExecute()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var user = await userManager.FindByNameAsync(Username);

                // Check if user already exists
                if (user != null)
                {
                    user.IsLockedOut = false;
                    var result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                        _logger.LogInformation($"User lockout disabled for {user.UserName}.");
                    else
                        foreach (var error in result.Errors)
                            _logger.LogError(error.Description);
                }
                // Create the new user
                else
                {
                    var result = await userManager.CreateAsync(new ApplicationUser {UserName = Username});

                    if (result.Succeeded)
                        _logger.LogInformation("User successfully created!");
                    else
                        foreach (var error in result.Errors)
                            _logger.LogError(error.Description);
                }
            }
        }
    }
}