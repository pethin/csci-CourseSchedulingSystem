using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Seeders;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Commands
{
    /// <summary>
    /// Command to seed the database.
    /// </summary>
    /// <remarks>By default this command runs the DatabaseSeeder seeder.</remarks>
    /// <param name="Seeder">(Optional) Seeder class name.</param>
    /// <param name="List">(Optional) Flag to list all available seeders.</param>
    [Command("seed", Description = "Seed the database", ExtendedHelpText = @"
Remarks:
  By default this command runs the DatabaseSeeder seeder.
")]
    [HelpOption("--help")]
    public class SeedCommand : ICommand
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public SeedCommand(IServiceProvider serviceProvider, ILogger<SeedCommand> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [Argument(0, "seeder", Description = "Run a specific seeder.")]
        public string Seeder { get; }

        [Option] public bool List { get; set; }

        public async Task OnExecute()
        {
            // If the list flag was set
            if (List)
            {
                // Get all classes that implement ISeeder interface in CourseSchedulingSystem.Data.Seeders namespace
                // and for each class print the class name.
                Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => type.IsClass && type.Namespace == typeof(DatabaseSeeder).Namespace)
                    .Where(type => type.GetInterfaces().Contains(typeof(ISeeder)))
                    .ToList()
                    .ForEach(seeder => Console.WriteLine(seeder.Name));
            }
            // If a specific seeder was specified
            else if (!string.IsNullOrWhiteSpace(Seeder))
            {
                // Try to find the seeder
                var seederType = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => type.IsClass && type.Namespace == typeof(DatabaseSeeder).Namespace)
                    .Where(type => type.GetInterfaces().Contains(typeof(ISeeder)))
                    .FirstOrDefault(type => type.Name == Seeder);

                if (seederType == null)
                {
                    _logger.LogError($"Could not find seeder: ${Seeder}");
                    throw new ArgumentException("Seeder not found.");
                }

                // Run the seeder
                await RunSeederAsync(_serviceProvider, seederType);
            }
            // If no specific seeder was specified
            else
            {
                // Run the DatabaseSeeder
                await RunSeederAsync(_serviceProvider, typeof(DatabaseSeeder));
            }
        }

        /// <summary>
        /// Runs the seeder in a new service provider scope.
        /// </summary>
        /// <param name="serviceProvider">The service provider to use.</param>
        /// <param name="seederType">The Type of the seeder.</param>
        /// <returns></returns>
        private async Task RunSeederAsync(IServiceProvider serviceProvider, Type seederType)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var seeder = (ISeeder) ActivatorUtilities.CreateInstance(scope.ServiceProvider, seederType);
                await seeder.SeedAsync();
            }
        }
    }
}