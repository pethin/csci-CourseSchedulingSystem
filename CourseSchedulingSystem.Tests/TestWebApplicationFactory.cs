using System;
using System.Reflection;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Seeders;
using CourseSchedulingSystem.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Tests
{
    public class TestWebApplicationFactory<TStartup>
        : WebApplicationFactory<CourseSchedulingSystem.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Add services
            builder.ConfigureServices(services =>
            {
            });

            // Replace services
            builder.ConfigureTestServices(services =>
            {
                // Replace the database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite("Data Source=:memory:;Version=3;New=True;");
                    });

                // Add ImpersonationController for authenticating test HTTP clients
                services
                    .AddMvcCore()
                    .AddApplicationPart(typeof(ImpersonationController).GetTypeInfo().Assembly);

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        var seeder = ActivatorUtilities.GetServiceOrCreateInstance<DatabaseSeeder>(scopedServices);
                        seeder.SeedAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex,
                            "An error occurred seeding the database with test messages. Error: {ex.Message}");
                    }
                }
            });

            base.ConfigureWebHost(builder);
        }
    }
}