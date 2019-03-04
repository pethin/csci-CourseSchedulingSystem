using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class CourseSchemaSeeder : ISeeder
    {
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
            await SeedAttributeTypesAsync();
        }

        /// <summary>
        /// Template for creating departments.
        /// </summary>
        private static readonly List<Department> DepartmentTemplates = new List<Department>
        {
            new Department("AFE", "Accounting, Finance, and Economics"),
            new Department("CSQM", "Computer Science & Quantitative Methods"),
            new Department("MM", "Management & Marketing")
        };

        /// <summary>
        /// Template for creating subjects.
        /// </summary>
        private static readonly List<Subject> SubjectTemplates = new List<Subject>
        {
            new Subject("ACCT", "Accounting"),
            new Subject("BADM", "Business Administration"),
            new Subject("CSCI", "Computer Science"),
            new Subject("DIFD", "Digital Information Design"),
            new Subject("ECON", "Economics"),
            new Subject("ENTR", "Entrepreneurship"),
            new Subject("FINC", "Finance"),
            new Subject("HCMT", "Health Care Management"),
            new Subject("MGMT", "Management"),
            new Subject("MKTG", "Marketing"),
            new Subject("QMTH", "Quantitative Methods")
        };

        /// <summary>
        /// List of schedule types to seed.
        /// </summary>
        private static readonly List<ScheduleType> ScheduleTypes = new List<ScheduleType>
        {
            new ScheduleType("Field Studies"),
            new ScheduleType("Independent Study/Research"),
            new ScheduleType("Internship"),
            new ScheduleType("Laboratory/Clinical"),
            new ScheduleType("Lecture"),
            new ScheduleType("Lecture/Lab-Clinical"),
            new ScheduleType("Practice Teaching"),
            new ScheduleType("Practice Cooperative edu"),
            new ScheduleType("Private Instruction"),
            new ScheduleType("Seminar/Recitation"),
            new ScheduleType("Studio/PE"),
            new ScheduleType("Thesis")
        };

        /// <summary>
        /// List of attribute types to seed.
        /// </summary>
        private static readonly List<AttributeType> AttributeTypes = new List<AttributeType>
        {
            new AttributeType("Capstone Course"),
            new AttributeType("Constitution Requirement"),
            new AttributeType("Global Perspective"),
            new AttributeType("Historical Perspective"),
            new AttributeType("Humanities and Arts"),
            new AttributeType("Intensive Writing"),
            new AttributeType("Logic Language and Semiotics"),
            new AttributeType("Natural Science - Earth"),
            new AttributeType("Natural Science - Life"),
            new AttributeType("Natural Science - Physical"),
            new AttributeType("Oral Communication"),
            new AttributeType("Physical Activity"),
            new AttributeType("Quantitative Skills Req"),
            new AttributeType("Social Science"),
            new AttributeType("Technology Requirement")
        };

        private async Task SeedDepartmentsAsync()
        {
            var departmentCodes = DepartmentTemplates.Select(d => d.Code).ToHashSet();
            var createdCodes = _context.Departments
                .Where(d => departmentCodes.Contains(d.Code))
                .Select(d => d.Code)
                .ToHashSet();

            var departments = DepartmentTemplates.Where(dt => !createdCodes.Contains(dt.Code));
            foreach (var department in departments)
            {
                await _context.Departments.AddAsync(department);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedSubjectsAsync()
        {
            var subjectCodes = SubjectTemplates.Select(d => d.Code).ToHashSet();
            var createdCodes = _context.Subjects
                .Where(s => subjectCodes.Contains(s.Code))
                .Select(s => s.Code)
                .ToHashSet();

            var subjects = SubjectTemplates.Where(dt => !createdCodes.Contains(dt.Code));
            foreach (var subject in subjects)
            {
                await _context.Subjects.AddAsync(subject);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedScheduleTypesAsync()
        {
            var scheduleTypeCodes = ScheduleTypes.Select(d => d.NormalizedName).ToHashSet();
            var createdCodes = _context.ScheduleTypes
                .Where(st => scheduleTypeCodes.Contains(st.NormalizedName))
                .Select(st => st.NormalizedName)
                .ToHashSet();

            var scheduleTypes = ScheduleTypes.Where(st => !createdCodes.Contains(st.NormalizedName));
            foreach (var scheduleType in scheduleTypes)
            {
                await _context.ScheduleTypes.AddAsync(scheduleType);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedAttributeTypesAsync()
        {
            var attributeTypeCodes = AttributeTypes.Select(d => d.NormalizedName).ToHashSet();
            var createdCodes = _context.AttributeTypes
                .Where(st => attributeTypeCodes.Contains(st.NormalizedName))
                .Select(st => st.NormalizedName)
                .ToHashSet();

            var attributeTypes = AttributeTypes.Where(st => !createdCodes.Contains(st.NormalizedName));
            foreach (var attributeType in attributeTypes)
            {
                await _context.AttributeTypes.AddAsync(attributeType);
            }

            await _context.SaveChangesAsync();
        }
    }
}