using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CourseSchedulingSystem.Commands
{
    [Command("loadcourses", Description = "Imports courses from Resources/Fixtures/Courses.xlsx")]
    [HelpOption("--help")]
    public class LoadCoursesCommand
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public LoadCoursesCommand(IServiceProvider serviceProvider, ILogger<LoadCoursesCommand> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task OnExecute()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await LoadExcelFixture(context);
            }
        }

        private async Task LoadExcelFixture(ApplicationDbContext context)
        {
            string coursesFixturePath = Path.Combine(PathUtilities.GetResourceDirectory(), "Fixtures", "Courses.xlsx");
            DataTable dt = new DataTable();

            using (var stream = new FileStream(coursesFixturePath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook hssfwb = new XSSFWorkbook(stream); // This will read 2007 Excel format

                ISheet sheet = hssfwb.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                IEnumerator rows = sheet.GetRowEnumerator();

                int colCount = headerRow.LastCellNum;

                // Read header row
                for (int c = 0; c < colCount; c++)
                {
                    dt.Columns.Add(headerRow.GetCell(c).ToString());
                }

                // Skip header row
                rows.MoveNext();
                while (rows.MoveNext())
                {
                    IRow row = (XSSFRow) rows.Current;
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < colCount; i++)
                    {
                        ICell cell = row.GetCell(i);

                        if (cell != null)
                        {
                            dr[i] = cell.ToString();
                        }
                    }

                    dt.Rows.Add(dr);
                }

                hssfwb.Close();
            }

            IList<CourseRow> courses = new List<CourseRow>();

            // Convert DataTable to list of row objects
            foreach (DataRow dr in dt.Rows)
            {
                var newCourse = new CourseRow
                {
                    DepartmentCode = dr["Department"].ToString().ToUpper(),
                    SubjectCode = dr["Subject"].ToString().ToUpper(),
                    Number = dr["Number"].ToString().ToUpper(),
                    Name = dr["Name"].ToString(),
                    CreditHours = Convert.ToDecimal(dr["Credit Hours"]),
                };

                var courseLevels = dr["Levels"].ToString().Split(",").Select(t => t.Trim()).ToHashSet();
                newCourse.IsGraduate = courseLevels.Contains("Graduate");
                newCourse.IsUndergraduate = courseLevels.Contains("Undergraduate");

                newCourse.ScheduleTypes = dr["Schedule Types"]
                    .ToString()
                    .Split(",")
                    .Select(st => st.Trim())
                    .Where(st => !string.IsNullOrEmpty(st))
                    .ToHashSet();
                newCourse.CourseAttributes = dr["Course Attributes"]
                    .ToString()
                    .Split(",")
                    .Select(ca => ca.Trim())
                    .Where(ca => !string.IsNullOrEmpty(ca))
                    .ToHashSet();

                courses.Add(newCourse);
            }

            var departments = await context.Departments.ToListAsync();
            var subjects = await context.Subjects.ToListAsync();
            var scheduleTypes = await context.ScheduleTypes.ToListAsync();
            var scheduleTypesNames = scheduleTypes.Select(st => st.Name).ToHashSet();
            var courseAttributes = await context.AttributeTypes.ToListAsync();
            var courseAttributesNames = courseAttributes.Select(ca => ca.Name).ToHashSet();

            foreach (var courseRow in courses)
            {
                var department = departments.FirstOrDefault(d => d.Code == courseRow.DepartmentCode);
                if (department == null)
                {
                    _logger.LogError(
                        $"Could not find department with code {courseRow.DepartmentCode} for {courseRow.Name}. Skipping...");
                    continue;
                }

                var subject = subjects.FirstOrDefault(s => s.Code == courseRow.SubjectCode);
                if (subject == null)
                {
                    _logger.LogError(
                        $"Could not find subject with code {courseRow.SubjectCode} for {courseRow.Name}. Skipping...");
                    continue;
                }

                if (courseRow.ScheduleTypes.Any(st => !scheduleTypesNames.Contains(st)))
                {
                    var missingScheduleTypes = string.Join(", ", courseRow.ScheduleTypes.Except(scheduleTypesNames));
                    _logger.LogError(
                        $"Could not find following schedule types for {courseRow.Name}: {missingScheduleTypes}. Skipping...");
                    continue;
                }

                if (courseRow.CourseAttributes.Any(ca => !courseAttributesNames.Contains(ca)))
                {
                    var missingAttributes = string.Join(", ", courseRow.CourseAttributes.Except(courseAttributesNames));
                    _logger.LogError(
                        $"Could not find following attributes for {courseRow.Name}: {missingAttributes}. Skipping...");
                    continue;
                }

                var course =
                    await context.Courses
                        .Include(c => c.CourseScheduleTypes)
                        .Include(c => c.CourseAttributeTypes)
                        .FirstOrDefaultAsync(c => c.SubjectId == subject.Id && c.Number == courseRow.Number);

                if (course != null)
                {
                    course.DepartmentId = department.Id;
                    course.Title = courseRow.Name;
                    course.CreditHours = courseRow.CreditHours;
                    course.IsGraduate = courseRow.IsGraduate;
                    course.IsUndergraduate = courseRow.IsUndergraduate;
                }
                else
                {
                    course = new Course
                    {
                        DepartmentId = department.Id,
                        SubjectId = subject.Id,
                        Number = courseRow.Number,
                        Title = courseRow.Name,
                        CreditHours = courseRow.CreditHours,
                        IsGraduate = courseRow.IsGraduate,
                        IsUndergraduate = courseRow.IsUndergraduate,
                        CourseScheduleTypes = new List<CourseScheduleType>(),
                        CourseAttributeTypes = new List<CourseAttributeType>()
                };
                    context.Courses.Add(course);
                }

                await context.SaveChangesAsync();

                // Update schedule types
                context.UpdateManyToMany(course.CourseScheduleTypes,
                    scheduleTypes
                        .Where(st => courseRow.ScheduleTypes.Contains(st.Name))
                        .Select(st => new CourseScheduleType
                        {
                            CourseId = course.Id,
                            ScheduleTypeId = st.Id
                        }),
                    cst => cst.ScheduleTypeId);

                // Update course attributes
                context.UpdateManyToMany(course.CourseAttributeTypes,
                    courseAttributes
                        .Where(ca => courseRow.CourseAttributes.Contains(ca.Name))
                        .Select(ca => new CourseAttributeType
                        {
                            CourseId = course.Id,
                            AttributeTypeId = ca.Id
                        }),
                    cat => cat.AttributeTypeId);

                await context.SaveChangesAsync();
            }
        }

        public class CourseRow
        {
            public string DepartmentCode { get; set; }
            public string SubjectCode { get; set; }
            public string Number { get; set; }
            public string Name { get; set; }
            public decimal CreditHours { get; set; }
            public bool IsGraduate { get; set; }
            public bool IsUndergraduate { get; set; }
            public ICollection<string> ScheduleTypes { get; set; }
            public ICollection<string> CourseAttributes { get; set; }
        }
    }
}