using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class DatabaseSeeder : ISeeder
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;

        public DatabaseSeeder(IServiceProvider provider, ILogger<DatabaseSeeder> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await CallAsync<IdentitySeeder>();
            await CallAsync<CourseSchemaSeeder>();
        }

        /// <summary>
        /// Runs a seeder.
        /// </summary>
        /// <param name="seeder"></param>
        /// <returns></returns>
        private async Task CallAsync<T>() where T : ISeeder
        {
            var seeder = ActivatorUtilities.GetServiceOrCreateInstance<T>(_provider);
            _logger.LogInformation($"Running {typeof(T).Name}");
            await seeder.SeedAsync();
        }
    }
}