using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Data.Seeders
{
    /// <summary>
    /// Seeds departments, subject, schedule types, and course attributes.
    /// </summary>
    public class CourseSchemaSeeder : ISeeder
    {
        /// <summary>
        ///     Template for creating departments.
        /// </summary>
        private static readonly List<Department> Departments = new List<Department>
        {
            new Department(Guid.Parse("00000000-0000-0000-0000-000000000001"), "ACFN",
                "Accounting, Finance, and Economics"),
            new Department(Guid.Parse("00000000-0000-0000-0000-000000000002"), "CSQM",
                "Computer Science & Quantitative Methods"),
            new Department(Guid.Parse("00000000-0000-0000-0000-000000000003"), "MGMK", "Management & Marketing")
        };

        /// <summary>
        ///     Template for creating subjects.
        /// </summary>
        private static readonly List<Subject> Subjects = new List<Subject>
        {
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000001"), "ACCT", "Accounting"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000002"), "BADM", "Business Administration"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000003"), "CSCI", "Computer Science"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000004"), "DIFD", "Digital Information Design"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000005"), "ECON", "Economics"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000006"), "ENTR", "Entrepreneurship"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000007"), "FINC", "Finance"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000008"), "HCMT", "Health Care Management"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-000000000009"), "MGMT", "Management"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-00000000000a"), "MKTG", "Marketing"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-00000000000b"), "QMTH", "Quantitative Methods"),
            new Subject(Guid.Parse("00000000-0000-0000-0000-00000000000c"), "SUBU", "Sustainability Studies")
        };

        /// <summary>
        ///     List of schedule types to seed.
        /// </summary>
        private static readonly List<ScheduleType> ScheduleTypes = new List<ScheduleType>
        {
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000001"), "FLD", "Field Studies"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000002"), "ISD", "Independent Study/Research"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000003"), "INT", "Internship"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000004"), "LAB", "Laboratory/Clinical"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000005"), "LEC", "Lecture"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000006"), "LLC", "Lecture/Lab-Clinical"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000007"), "PRT", "Practice Teaching"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000008"), "PRC", "Practicum/Cooperative edu"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-000000000009"), "PRI", "Private Instruction"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-00000000000a"), "SEM", "Seminar/Recitation"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-00000000000b"), "STU", "Studio/PE"),
            new ScheduleType(Guid.Parse("00000000-0000-0000-0000-00000000000c"), "THS", "Thesis")
        };

        /// <summary>
        ///     List of course attributes to seed.
        /// </summary>
        private static readonly List<CourseAttribute> CourseAttributes = new List<CourseAttribute>
        {
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Capstone Course"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Constitution Requirement"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000003"), "Global Perspective"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000004"), "Historical Perspective"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000005"), "Humanities and Arts"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000006"), "Intensive Writing"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000007"), "Logic Language and Semiotics"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000008"), "Natural Science - Earth"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-000000000009"), "Natural Science - Life"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-00000000000a"), "Natural Science - Physical"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-00000000000b"), "Oral Communication"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-00000000000c"), "Physical Activity"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-00000000000d"), "Quantitative Skills Req"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-00000000000e"), "Social Science"),
            new CourseAttribute(Guid.Parse("00000000-0000-0000-0000-00000000000f"), "Technology Requirement")
        };

        private readonly ApplicationDbContext _context;

        public CourseSchemaSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedDepartmentsAsync();
            await SeedSubjectsAsync();
            await SeedScheduleTypesAsync();
            await SeedCourseAttributesAsync();
        }

        private async Task SeedDepartmentsAsync()
        {
            var templateIds = Departments.Select(d => d.Id).ToHashSet();
            var createdTemplatesIds = _context.Departments
                .Where(d => templateIds.Contains(d.Id))
                .Select(d => d.Id)
                .ToHashSet();

            var departments = Departments.Where(dt => !createdTemplatesIds.Contains(dt.Id)).ToList();
            _context.Departments.AddRange(departments);

            await _context.SaveChangesAsync();
        }

        private async Task SeedSubjectsAsync()
        {
            var templateIds = Subjects.Select(s => s.Id).ToHashSet();
            var createdTemplatesIds = _context.Subjects
                .Where(s => templateIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToHashSet();

            var subjects = Subjects.Where(st => !createdTemplatesIds.Contains(st.Id)).ToList();
            _context.Subjects.AddRange(subjects);

            await _context.SaveChangesAsync();
        }

        private async Task SeedScheduleTypesAsync()
        {
            var templateIds = ScheduleTypes.Select(t => t.Id).ToHashSet();
            var createdTemplatesIds = _context.ScheduleTypes
                .Where(m => templateIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToHashSet();

            var scheduleTypes = ScheduleTypes.Where(t => !createdTemplatesIds.Contains(t.Id)).ToList();
            _context.ScheduleTypes.AddRange(scheduleTypes);

            await _context.SaveChangesAsync();
        }

        private async Task SeedCourseAttributesAsync()
        {
            var templateIds = CourseAttributes.Select(t => t.Id).ToHashSet();
            var createdTemplatesIds = _context.CourseAttributes
                .Where(m => templateIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToHashSet();

            var courseAttributes = CourseAttributes.Where(t => !createdTemplatesIds.Contains(t.Id)).ToList();
            _context.CourseAttributes.AddRange(courseAttributes);

            await _context.SaveChangesAsync();
        }
    }
}