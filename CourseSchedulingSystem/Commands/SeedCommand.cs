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
            if (List)
            {
                Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => type.IsClass && type.Namespace == typeof(DatabaseSeeder).Namespace)
                    .Where(type => type.GetInterfaces().Contains(typeof(ISeeder)))
                    .ToList()
                    .ForEach(seeder => Console.WriteLine(seeder.Name));
            }
            else if (Seeder != null)
            {
                var seederClassPath = $"{typeof(DatabaseSeeder).Namespace}.{Seeder}";
                var seederType = Type.GetType(seederClassPath);

                if (seederType == null)
                {
                    _logger.LogError($"Could not find seeder: {seederClassPath}");
                    throw new ArgumentException("Seeder not found.");
                }

                await RunSeederAsync(_serviceProvider, seederType);
            }
            else
            {
                await RunSeederAsync(_serviceProvider, typeof(DatabaseSeeder));
            }
        }

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