using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CourseSchedulingSystem.Services.TestingAuthentication
{
    public static class TestingAuthenticationExtensions
    {
        public static void AddTesting<TKey, TUser, TGroup>(this AuthenticationOptions options)
            where TKey : IEquatable<TKey>
            where TUser : IdentityUser<TKey>, new()
            where TGroup : IdentityRole<TKey>, new()
        {
            options.DefaultAuthenticateScheme = TestingAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = TestingAuthenticationDefaults.AuthenticationScheme;
            
            options.AddScheme<TestingAuthenticationHandler<TKey, TUser, TGroup>>(
                TestingAuthenticationDefaults.AuthenticationScheme,
                TestingAuthenticationDefaults.AuthenticationScheme);
        }

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

        public static AuthenticationBuilder AddTesting<TKey, TUser, TGroup>(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<TestingAuthenticationOptions> configureOptions)
            where TKey : IEquatable<TKey>
            where TUser : IdentityUser<TKey>, new()
            where TGroup : IdentityRole<TKey>, new()
        {
            return builder.AddScheme<TestingAuthenticationOptions, TestingAuthenticationHandler<TKey, TUser, TGroup>>(
                authenticationScheme, configureOptions);
        }
    }
}