using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CourseSchedulingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserClaim> UserClaims { get; set; }

        public DbSet<ExternalUserLogin> ExternalUserLogins { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<RoleClaim> RoleClaims { get; set; }

        private StoreOptions GetStoreOptions() => this.GetService<IDbContextOptions>()
            .Extensions.OfType<CoreOptionsExtension>()
            .FirstOrDefault()?.ApplicationServiceProvider
            ?.GetService<IOptions<IdentityOptions>>()
            ?.Value?.Stores;

        private class PersonalDataConverter : ValueConverter<string, string>
        {
            public PersonalDataConverter(IPersonalDataProtector protector) : base(s => protector.Protect(s),
                s => protector.Unprotect(s), default(ConverterMappingHints))
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var storeOptions = GetStoreOptions();
            var maxKeyLength = storeOptions?.MaxLengthForKeys ?? 0;
            var encryptPersonalData = storeOptions?.ProtectPersonalData ?? false;
            PersonalDataConverter converter = null;

            builder.Entity<User>(b =>
            {
                b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);

                if (encryptPersonalData)
                {
                    converter = new PersonalDataConverter(this.GetService<IPersonalDataProtector>());

                    var personalDataProps = typeof(User).GetProperties()
                        .Where(prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));

                    foreach (var p in personalDataProps)
                    {
                        if (p.PropertyType != typeof(string))
                        {
                            throw new InvalidOperationException(
                                "[ProtectedPersonalData] only works strings by default.");
                        }

                        b.Property(typeof(string), p.Name).HasConversion(converter);
                    }
                }

                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany<ExternalUserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
            });

            builder.Entity<ExternalUserLogin>(b =>
            {
                b.HasKey(l => new {l.LoginProvider, l.ProviderKey});

                if (maxKeyLength > 0)
                {
                    b.Property(l => l.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(l => l.ProviderKey).HasMaxLength(maxKeyLength);
                }
            });

            builder.Entity<UserRole>(b => { b.HasKey(r => new {r.UserId, r.RoleId}); });

            builder.Entity<Role>(b =>
            {
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });
        }
    }
}