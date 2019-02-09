using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class DatabaseSeeder : ISeeder
    {
        private readonly IServiceProvider _provider;

        public DatabaseSeeder(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task SeedAsync()
        {
            await CallAsync<IdentitySeeder>();
        }

        /// <summary>
        /// Runs a seeder.
        /// </summary>
        /// <param name="seeder"></param>
        /// <returns></returns>
        private async Task CallAsync<T>() where T : ISeeder
        {
            var seeder = ActivatorUtilities.GetServiceOrCreateInstance<T>(_provider);
            await seeder.SeedAsync();
        }
    }
}