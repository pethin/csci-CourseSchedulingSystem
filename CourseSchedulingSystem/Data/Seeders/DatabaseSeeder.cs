﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Data.Seeders
{
    /// <summary>
    /// The default seeder.
    /// </summary>
    public class DatabaseSeeder : ISeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IServiceProvider _provider;

        public DatabaseSeeder(IServiceProvider provider, ILogger<DatabaseSeeder> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await CallAsync<IdentitySeeder>();
            await CallAsync<CourseSchemaSeeder>();
            await CallAsync<InfrastructureSeeder>();
            await CallAsync<SchedulingSchemaSeeder>();
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