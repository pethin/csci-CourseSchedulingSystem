using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Commands
{
    [Command("migrate", Description = "Updates the database to a specified migration.")]
    [HelpOption("--help")]
    public class Migrate
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public Migrate(IServiceProvider serviceProvider, ILogger<Migrate> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [Argument(0, "target", Description = "The target migration. If '0', all migrations will be reverted.")]
        public string Target { get; }

        [Option("--seed", Description = "Seed the database after migrations are run.")]
        public bool Seed { get; set; }

        [Option("--script", Description =
            "Generate migrations script. Use --from and --to to select specific migrations.")]
        public bool Script { get; set; }

        [Option("--from", Description =
            "Used when generating an SQL script. The migration to start from. Defaults to the empty database.")]
        public string From { get; }

        [Option("--to", Description =
            "Used when generating an SQL script. The target migration to migrate the database. Defaults to the latest migration.")]
        public string To { get; }

        [Option("--idempotent", Description =
            "The generated script only applies migrations if they haven't already been applied to the database. Defaults to true.")]
        public bool Idempotent { get; set; } = true;

        public async Task OnExecute()
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var migrator = dbContext.GetService<IMigrator>();

                // If script option is true, generate scripts
                if (Script)
                {
                    Console.WriteLine(migrator.GenerateScript(From, To, Idempotent));
                }
                // Otherwise, run migrations
                else
                {
                    _logger.LogInformation("Running migrations.");
                    await migrator.MigrateAsync(Target);

                    _logger.LogInformation("Seeding database.");
                    if (Seed)
                    {
                        var seedCommand = ActivatorUtilities.CreateInstance<Seed>(serviceScope.ServiceProvider);
                        await seedCommand.OnExecute();
                    }
                }
            }
        }
    }
}