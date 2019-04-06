using System.Collections.Generic;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class DevelopmentSeeder : ISeeder
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;

        public DevelopmentSeeder(
            IHostingEnvironment env,
            ILogger<DevelopmentSeeder> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await Task.Yield();

            if (!_env.IsDevelopment())
            {
                _logger.LogWarning("Skipping... This seeder can only be run in development env.");
            }
        }
    }
}