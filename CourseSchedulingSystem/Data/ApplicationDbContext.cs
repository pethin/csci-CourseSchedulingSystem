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
        public DbSet<CourseAttribute> CourseAttributes { get; set; }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseScheduleType> CourseScheduleTypes { get; set; }
        public DbSet<CourseCourseAttribute> CourseCourseAttributes { get; set; }

        public DbSet<Term> Terms { get; set; }
        public DbSet<TermPart> TermParts { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<InstructionalMethod> InstructionalMethods { get; set; }
        public DbSet<Building> Building { get; set; }
        public DbSet<Room> Room { get; set; }

        public DbSet<Capability> Capability { get; set; }

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
                    .HasForeignKey(du => du.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
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
            builder.Entity<CourseAttribute>()
                .HasIndex(at => at.NormalizedName)
                .IsUnique();

            // Course
            builder.Entity<Course>(b =>
            {
                // Course >>-- Subject
                b.HasIndex(c => new {c.SubjectId, Level = c.Number}).IsUnique();

                b.HasOne(c => c.Department)
                    .WithMany(d => d.Courses)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(c => c.Subject)
                    .WithMany(s => s.Courses)
                    .OnDelete(DeleteBehavior.Restrict);
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
                    .HasForeignKey(cst => cst.ScheduleTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Course >>--<< CourseAttribute
            builder.Entity<CourseCourseAttribute>(b =>
            {
                b.HasKey(t => new {t.CourseId, t.CourseAttributeId});

                b.HasOne(cat => cat.Course)
                    .WithMany(c => c.CourseCourseAttributes)
                    .HasForeignKey(cat => cat.CourseId);

                b.HasOne(cat => cat.CourseAttribute)
                    .WithMany(at => at.CourseCourseAttributes)
                    .HasForeignKey(cat => cat.CourseAttributeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Term
            builder.Entity<Term>()
                .HasIndex(t => t.NormalizedName)
                .IsUnique();

            // TermPart >>-- Term
            builder.Entity<TermPart>(b =>
            {
                b.HasIndex(c => new { c.TermId, c.NormalizedName }).IsUnique();

                b.HasOne(tp => tp.Term)
                    .WithMany(t => t.TermParts);
            });
            
            // Instructor
            builder.Entity<Instructor>()
                .HasIndex(i => i.NormalizedName)
                .IsUnique();

            // InstructionalMethod
            builder.Entity<InstructionalMethod>()
                .HasIndex(im => im.NormalizedName)
                .IsUnique();

            // Building
            builder.Entity<Building>(b =>
            {
                b.HasIndex(s => s.Code).IsUnique();
                b.HasIndex(s => s.NormalizedName).IsUnique();
            });

            // Room
            builder.Entity<Room>()
                //Room >>-- Building
                .HasIndex(rm => new { rm.BuildingId, rm.Number })
                .IsUnique();

            // Capabilities
            builder.Entity<Capability>()
               .HasIndex(cp => cp.NormalizedName)
               .IsUnique();
        }

    }
}