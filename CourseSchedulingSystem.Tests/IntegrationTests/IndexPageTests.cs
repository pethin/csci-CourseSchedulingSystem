using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CourseSchedulingSystem.Models;
using CourseSchedulingSystem.Tests.Factories;
using CourseSchedulingSystem.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using HttpClientExtensions = CourseSchedulingSystem.Tests.Helpers.HttpClientExtensions;

namespace CourseSchedulingSystem.Tests.IntegrationTests
{
    public class IndexPageTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> _factory;

        public IndexPageTests(TestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        public async Task Get_RedirectsToLoginIfNotAuthenticated(string url)
        {
            // Act
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.StartsWith("/Identity/Account/Login",
                response.RequestMessage.RequestUri.PathAndQuery);
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        public async Task Get_RedirectsToManageIfAuthenticated(string url)
        {
            // Act
            var user = new UserFactory().Generate();

            // CreateClient instantiates _factory.Server
            var client = _factory.CreateClient();
            using (var scope = _factory.Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var result = await userManager.CreateAsync(user);
                Assert.True(result.Succeeded);
            }

            await client.ActAsAsync(user);

            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.StartsWith("/Manage",
                response.RequestMessage.RequestUri.PathAndQuery);
        }
    }
}