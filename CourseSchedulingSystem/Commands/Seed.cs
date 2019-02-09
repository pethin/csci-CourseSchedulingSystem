using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Seeders;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Commands
{
    [Command("seed")]
    [HelpOption("--help")]
    public class Seed
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public Seed(IServiceProvider serviceProvider, ILogger<Seed> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [Argument(0, Name = "seeder", Description = "Run a specific seeder.")]
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

                var seeder = (ISeeder) ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, seederType);

                await seeder.SeedAsync();
            }
            else
            {
                var seeder = ActivatorUtilities.GetServiceOrCreateInstance<DatabaseSeeder>(_serviceProvider);
                await seeder.SeedAsync();
            }
        }
    }
}