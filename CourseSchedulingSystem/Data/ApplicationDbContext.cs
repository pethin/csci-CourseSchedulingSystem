using System;
using CourseSchedulingSystem.Data.Models;
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

        public DbSet<Term> Terms { get; set; }
        public DbSet<TermPart> TermParts { get; set; }

        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<InstructionalMethod> InstructionalMethods { get; set; }
        public DbSet<Building> Building { get; set; }
        public DbSet<Room> Room { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User >>--<< Department
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

            // Department
            builder.Entity<Department>(b =>
            {
                b.HasIndex(d => d.Code).IsUnique();
                b.HasIndex(d => d.NormalizedName).IsUnique();
            });

            // Subject
            builder.Entity<Subject>(b =>
            {
                b.HasIndex(s => s.Code).IsUnique();
                b.HasIndex(s => s.NormalizedName).IsUnique();
            });

            // Schedule Type
            builder.Entity<ScheduleType>()
                .HasIndex(st => st.NormalizedName)
                .IsUnique();

            // Attribute Type
            builder.Entity<AttributeType>()
                .HasIndex(at => at.NormalizedName)
                .IsUnique();

            // Course
            builder.Entity<Course>(b =>
            {
                // Course >>-- Subject
                b.HasIndex(c => new {c.SubjectId, Level = c.Number}).IsUnique();
            });

            // Course >>--<< ScheduleType
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

            // Course >>--<< AttributeType
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

            // Term
            builder.Entity<Term>()
                .HasIndex(t => t.NormalizedName)
                .IsUnique();

            // TermPart >>-- Term
            builder.Entity<TermPart>()
                .HasIndex(c => new {c.TermId, c.NormalizedName})
                .IsUnique();

            // Instructor
            builder.Entity<Instructor>()
                .HasIndex(i => i.NormalizedName)
                .IsUnique();

            // InstructionalMethod
            builder.Entity<InstructionalMethod>()
                .HasIndex(im => im.NormalizedName)
                .IsUnique();

            // Building
            builder.Entity<Building>()
                .HasIndex(bd => bd.NormalizedName)
                .IsUnique();

            // Room
            builder.Entity<Room>()
                //Room >>-- Building
                .HasIndex(rm => new { rm.BuildingId, rm.Number })
                .IsUnique();
        }

    }
}