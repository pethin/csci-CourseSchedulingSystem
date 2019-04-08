using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class CourseSchemaSeeder : ISeeder
    {
        /// <summary>
        ///     Template for creating departments.
        /// </summary>
        private static readonly List<Department> DepartmentTemplates = new List<Department>
        {
            new Department("ACFN", "Accounting, Finance, and Economics"),
            new Department("CSQM", "Computer Science & Quantitative Methods"),
            new Department("MGMK", "Management & Marketing")
        };

        /// <summary>
        ///     Template for creating subjects.
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
            new Subject("QMTH", "Quantitative Methods"),
            new Subject("SUBU", "Sustainability Studies")
        };

        /// <summary>
        ///     List of schedule types to seed.
        /// </summary>
        private static readonly List<ScheduleType> ScheduleTypes = new List<ScheduleType>
        {
            new ScheduleType("FLD", "Field Studies"),
            new ScheduleType("ISD", "Independent Study/Research"),
            new ScheduleType("INT", "Internship"),
            new ScheduleType("LAB", "Laboratory/Clinical"),
            new ScheduleType("LEC", "Lecture"),
            new ScheduleType("LLC", "Lecture/Lab-Clinical"),
            new ScheduleType("PRT", "Practice Teaching"),
            new ScheduleType("PRC", "Practicum/Cooperative edu"),
            new ScheduleType("PRI", "Private Instruction"),
            new ScheduleType("SEM", "Seminar/Recitation"),
            new ScheduleType("STU", "Studio/PE"),
            new ScheduleType("THS", "Thesis")
        };

        /// <summary>
        ///     List of course attributes to seed.
        /// </summary>
        private static readonly List<CourseAttribute> AttributeTypes = new List<CourseAttribute>
        {
            new CourseAttribute("Capstone Course"),
            new CourseAttribute("Constitution Requirement"),
            new CourseAttribute("Global Perspective"),
            new CourseAttribute("Historical Perspective"),
            new CourseAttribute("Humanities and Arts"),
            new CourseAttribute("Intensive Writing"),
            new CourseAttribute("Logic Language and Semiotics"),
            new CourseAttribute("Natural Science - Earth"),
            new CourseAttribute("Natural Science - Life"),
            new CourseAttribute("Natural Science - Physical"),
            new CourseAttribute("Oral Communication"),
            new CourseAttribute("Physical Activity"),
            new CourseAttribute("Quantitative Skills Req"),
            new CourseAttribute("Social Science"),
            new CourseAttribute("Technology Requirement")
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
            await SeedAttributeTypesAsync();
        }

        private async Task SeedDepartmentsAsync()
        {
            var departmentCodes = DepartmentTemplates.Select(d => d.Code).ToHashSet();
            var createdCodes = _context.Departments
                .Where(d => departmentCodes.Contains(d.Code))
                .Select(d => d.Code)
                .ToHashSet();

            var departments = DepartmentTemplates.Where(dt => !createdCodes.Contains(dt.Code));
            foreach (var department in departments) await _context.Departments.AddAsync(department);

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
            foreach (var subject in subjects) await _context.Subjects.AddAsync(subject);

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
            foreach (var scheduleType in scheduleTypes) await _context.ScheduleTypes.AddAsync(scheduleType);

            await _context.SaveChangesAsync();
        }

        private async Task SeedAttributeTypesAsync()
        {
            var attributeTypeCodes = AttributeTypes.Select(d => d.NormalizedName).ToHashSet();
            var createdCodes = _context.CourseAttributes
                .Where(st => attributeTypeCodes.Contains(st.NormalizedName))
                .Select(st => st.NormalizedName)
                .ToHashSet();

            var attributeTypes = AttributeTypes.Where(st => !createdCodes.Contains(st.NormalizedName));
            foreach (var attributeType in attributeTypes) await _context.CourseAttributes.AddAsync(attributeType);

            await _context.SaveChangesAsync();
        }
    }
}