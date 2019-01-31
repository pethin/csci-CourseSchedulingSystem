using System;
using System.Collections.Generic;
using System.Text;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public static readonly Guid AdminRoleId = new Guid("00000000-0000-0000-0000-000000000042");

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole
                {
                    Id = AdminRoleId,
                    Name = "Administrator",
                    NormalizedName = "Administrator".ToUpper(),
                    Description = "Grants all permissions",
                    ConcurrencyStamp = "1348ef0a-5677-4733-8cfa-0a9095de0f28"
                });

            // Create admin user with UserName = "admin" and Password = "password"
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = AdminRoleId,
                    UserName = "admin",
                    NormalizedUserName = "admin".ToUpper(),
                    Email = "admin@winthrop.edu",
                    NormalizedEmail = "admin@winthrop.edu".ToUpper(),
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAEGmIQHSiQtppRL+j/nV+gfDyJo3BALbu1e6u+bg+RU/4bO7e2Iovgzw/oFVN5goYRw==",
                    SecurityStamp = string.Empty,
                    ConcurrencyStamp = "b5014cf6-cfb9-43d5-9bff-c9211fc5ce7d"
                });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    RoleId = AdminRoleId,
                    UserId = AdminRoleId
                });
        }
    }
}