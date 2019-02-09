using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Commands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem
{
    [Command("CourseSchedulingSystem", ExtendedHelpText = @"
Remarks:
  The server will start if no sub-command is specified.
")]
    [HelpOption("--help")]
    [Subcommand(typeof(Seed), typeof(CreateAdminUser))]
    public class Program
    {
        private static IWebHost _host;

        public static int Main(string[] args)
        {
            _host = CreateWebHostBuilder(args).Build();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(_host.Services);
            return app.Execute(args);
        }

        public async Task OnExecute()
        {
            await _host.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}