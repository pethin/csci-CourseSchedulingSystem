using System;
using System.Collections.Generic;
using System.Text;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentUser> DepartmentUsers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        
        public DbSet<ScheduleType> ScheduleTypes { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseScheduleType> CourseScheduleTypes { get; set; }
        public DbSet<CourseAttributeType> CourseAttributeTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
            .UseLazyLoadingProxies();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User <<-->> Department
            builder.Entity<DepartmentUser>(b =>
            {
                b.HasKey(t => new {t.UserId, t.DepartmentId});

                b.HasOne(du => du.User)
                    .WithMany(u => u.DepartmentUsers)
                    .HasForeignKey(du => du.UserId);

                b.HasOne(du => du.Department)
                    .WithMany(d => d.DepartmentUsers)
                    .HasForeignKey(du => du.DepartmentId);
            });

            builder.Entity<Department>(b =>
            {
                b.HasIndex(d => d.Code).IsUnique();
                b.HasIndex(d => d.NormalizedName).IsUnique();
            });

            builder.Entity<Subject>(b =>
            {
                b.HasIndex(s => s.Code).IsUnique();
                b.HasIndex(s => s.NormalizedName).IsUnique();
            });

            builder.Entity<ScheduleType>()
                .HasIndex(st => st.NormalizedName)
                .IsUnique();

            builder.Entity<AttributeType>()
                .HasIndex(at => at.NormalizedName)
                .IsUnique();

            builder.Entity<Course>(b =>
            {
                b.HasIndex(c => new { c.SubjectId, c.Level }).IsUnique();
                b.HasIndex(c => c.NormalizedTitle).IsUnique();
            });

            // Course <<-->> ScheduleType
            builder.Entity<CourseScheduleType>(b =>
            {
                b.HasKey(t => new {t.CourseId, t.ScheduleTypeId});

                b.HasOne(cst => cst.Course)
                    .WithMany(c => c.CourseScheduleTypes)
                    .HasForeignKey(cst => cst.CourseId);

                b.HasOne(cst => cst.ScheduleType)
                    .WithMany(st => st.CourseScheduleTypes)
                    .HasForeignKey(cst => cst.ScheduleTypeId);
            });

            // Course <<-->> AttributeType
            builder.Entity<CourseAttributeType>(b =>
            {
                b.HasKey(t => new {t.CourseId, t.AttributeTypeId});

                b.HasOne(cat => cat.Course)
                    .WithMany(c => c.CourseAttributeTypes)
                    .HasForeignKey(cat => cat.CourseId);

                b.HasOne(cat => cat.AttributeType)
                    .WithMany(at => at.CourseAttributeTypes)
                    .HasForeignKey(cat => cat.AttributeTypeId);
            });
        }
    }
}