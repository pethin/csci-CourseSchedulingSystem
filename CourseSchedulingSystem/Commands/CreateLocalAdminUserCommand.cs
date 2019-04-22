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
    /// <summary>
    /// Creates a local admin user. The user can be logged in at "/Identity/Account/LocalLogin".
    /// </summary>
    /// <remarks>Will prompt user for password.</remarks>
    /// <param name="Username">The username of the user.</param>
    [Command("createlocaladminuser", Description = "Create an local Administrator user. Login page at /Identity/Account/LocalLogin")]
    [HelpOption("--help")]
    public class CreateLocalAdminUserCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public CreateLocalAdminUserCommand(IServiceProvider serviceProvider, ILogger<CreateLocalAdminUserCommand> logger)
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
                    _logger.LogError($"User with user name, {Username}, already exists!");
                }
                // Create the new user
                else
                {
                    var password = ReadPasswordWithConfirm();
                    var result = await userManager.CreateAsync(new ApplicationUser {UserName = Username}, password);

                    if (result.Succeeded)
                        _logger.LogInformation("User successfully created!");
                    else
                        foreach (var error in result.Errors)
                            _logger.LogError(error.Description);
                }
            }
        }

        private string ReadPasswordWithConfirm()
        {
            string password, confirmPassword;

            do
            {
                password = ReadPassword("New password: ");
                confirmPassword = ReadPassword("Retype new password: ");

                if (password != confirmPassword) Console.WriteLine("Passwords do not match!");
            } while (password != confirmPassword);

            return password;
        }

        private string ReadPassword(string prompt = "Password: ")
        {
            // Instantiate the secure string.
            var password = "";
            ConsoleKeyInfo key;

            while (password.Length == 0)
            {
                Console.Write(prompt);
                do
                {
                    key = Console.ReadKey(true);

                    // Ignore any key out of range.
                    if (!char.IsControl(key.KeyChar))
                    {
                        // Append the character to the password.
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (password.Length > 0)
                        {
                            password = password.Remove(password.Length - 1);
                            Console.Write("\b \b");
                        }
                    }

                    // Exit if Enter key is pressed.
                } while (key.Key != ConsoleKey.Enter);
            }

            Console.WriteLine();

            return password;
        }
    }
}