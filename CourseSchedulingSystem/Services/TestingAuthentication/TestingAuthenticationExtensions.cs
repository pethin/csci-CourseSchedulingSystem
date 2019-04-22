using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CourseSchedulingSystem.Services.TestingAuthentication
{
    public static class TestingAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTesting<TKey, TUser, TGroup>(this AuthenticationBuilder builder)
            where TKey : IEquatable<TKey>
            where TUser : IdentityUser<TKey>, new()
            where TGroup : IdentityRole<TKey>, new()
        {
            return AddTesting<TKey, TUser, TGroup>(builder, TestingAuthenticationDefaults.AuthenticationScheme,
                _ => { });
        }

        public static AuthenticationBuilder AddTesting<TKey, TUser, TGroup>(
            this AuthenticationBuilder builder,
            string authenticationScheme)
            where TKey : IEquatable<TKey>
            where TUser : IdentityUser<TKey>, new()
            where TGroup : IdentityRole<TKey>, new()
        {
            return AddTesting<TKey, TUser, TGroup>(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddTesting<TKey, TUser, TGroup>(
            this AuthenticationBuilder builder,
            Action<TestingAuthenticationOptions> configureOptions)
            where TKey : IEquatable<TKey>
            where TUser : IdentityUser<TKey>, new()
            where TGroup : IdentityRole<TKey>, new()
        {
            return AddTesting<TKey, TUser, TGroup>(builder, TestingAuthenticationDefaults.AuthenticationScheme,
                configureOptions);
        }

        /// <summary>
        /// Adds the TestingOrIdentity scheme. If the "Authorization" header is detected in the request,
        /// the TestingAuthentication handler will be executed. Otherwise, the ASP.NET Core Identity handler will be
        /// called. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authenticationScheme"></param>
        /// <param name="configureOptions"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TGroup"></typeparam>
        /// <returns></returns>
        public static AuthenticationBuilder AddTesting<TKey, TUser, TGroup>(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<TestingAuthenticationOptions> configureOptions)
            where TKey : IEquatable<TKey>
            where TUser : IdentityUser<TKey>, new()
            where TGroup : IdentityRole<TKey>, new()
        {
            return builder
                .AddPolicyScheme(
                    TestingAuthenticationDefaults.IdentityFallbackScheme,
                    TestingAuthenticationDefaults.IdentityFallbackScheme,
                    options =>
                    {
                        options.ForwardDefaultSelector = context =>
                        {
                            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                            if (authHeader != null)
                            {
                                return TestingAuthenticationDefaults.AuthenticationScheme;
                            }

                            return IdentityConstants.ApplicationScheme;
                        };
                    })
                .AddScheme<TestingAuthenticationOptions, TestingAuthenticationHandler<TKey, TUser, TGroup>>(
                    authenticationScheme, configureOptions);
        }
    }
}