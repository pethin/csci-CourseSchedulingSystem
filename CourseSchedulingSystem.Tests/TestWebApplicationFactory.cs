using System;
using System.Reflection;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Tests
{
    public class TestWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        private readonly SqliteConnection _connection;

        // Flag: Has Dispose already been called?
        private bool _disposed;

        public TestWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Set env to testing
            builder.UseEnvironment("Testing");

            // Add services
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkSqlite()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                    options.UseInternalServiceProvider(serviceProvider);
                });

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
                    logger.LogError("Creating schema");
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
//                        var seeder = ActivatorUtilities.GetServiceOrCreateInstance<DatabaseSeeder>(scopedServices);
//                        seeder.SeedAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex,
                            "An error occurred seeding the database with test messages. Error: {ex.Message}");
                    }
                }
            });

            // Replace services
            builder.ConfigureTestServices(services =>
            {
            });

            base.ConfigureWebHost(builder);
        }

        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing) _connection.Close();

            _disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}