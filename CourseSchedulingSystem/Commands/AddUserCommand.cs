using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == Username.Trim().ToUpper());

                // Check if user already exists
                if (user != null)
                {
                    user.IsLockedOut = false;
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User lockout disabled for {user.UserName}.");
                }
                // Create the new user
                else
                {
                    var newUser = new User
                    {
                        UserName = Username
                    };
                    dbContext.Users.Add(newUser);
                    await dbContext.SaveChangesAsync();
                    
                    _logger.LogInformation("User successfully created!");
                }
            }
        }
    }
}