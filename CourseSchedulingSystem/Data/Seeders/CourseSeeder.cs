using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class CourseSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CourseSeeder> _logger;

        public CourseSeeder(ApplicationDbContext context, ILogger<CourseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            var dataTable = LoadExcelFixture();
            var courseRows = ConvertDataTableToCourseRows(dataTable);
            await InsertCourseRows(courseRows);
        }

        private DataTable LoadExcelFixture()
        {
            var coursesFixturePath = Path.Combine(PathUtilities.GetResourceDirectory(), "Fixtures", "Courses.xlsx");
            var dt = new DataTable();

            using (var stream = new FileStream(coursesFixturePath, FileMode.Open, FileAccess.Read))
            {
                var hssfwb = new XSSFWorkbook(stream); // This will read 2007 Excel format

                var sheet = hssfwb.GetSheetAt(0);
                var headerRow = sheet.GetRow(0);
                var rows = sheet.GetRowEnumerator();

                int colCount = headerRow.LastCellNum;

                // Read header row
                for (var c = 0; c < colCount; c++) dt.Columns.Add(headerRow.GetCell(c).ToString());

                // Skip header row
                rows.MoveNext();
                while (rows.MoveNext())
                {
                    IRow row = (XSSFRow) rows.Current;
                    var dr = dt.NewRow();

                    for (var i = 0; i < colCount; i++)
                    {
                        var cell = row.GetCell(i);

                        if (cell != null) dr[i] = cell.ToString();
                    }

                    dt.Rows.Add(dr);
                }

                hssfwb.Close();
            }

            return dt;
        }

        private ICollection<CourseRow> ConvertDataTableToCourseRows(DataTable dt)
        {
            IList<CourseRow> courses = new List<CourseRow>();

            foreach (DataRow dr in dt.Rows)
            {
                var newCourse = new CourseRow
                {
                    DepartmentCode = dr["Department"].ToString().ToUpper(),
                    SubjectCode = dr["Subject"].ToString().ToUpper(),
                    Number = dr["Number"].ToString().ToUpper(),
                    Name = dr["Name"].ToString(),
                    CreditHours = Convert.ToDecimal(dr["Credit Hours"])
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

            return courses;
        }

        private async Task InsertCourseRows(ICollection<CourseRow> courseRows)
        {
            var departments = await _context.Departments.ToListAsync();
            var subjects = await _context.Subjects.ToListAsync();
            var scheduleTypes = await _context.ScheduleTypes.ToListAsync();
            var scheduleTypesNames = scheduleTypes.Select(st => st.Name).ToHashSet();
            var courseAttributes = await _context.CourseAttributes.ToListAsync();
            var courseAttributesNames = courseAttributes.Select(ca => ca.Name).ToHashSet();

            var errors = new List<string>();
            
            foreach (var courseRow in courseRows)
            {
                var department = departments.FirstOrDefault(d => d.Code == courseRow.DepartmentCode);
                if (department == null)
                {
                    var error = $"Could not find department with code {courseRow.DepartmentCode} for {courseRow.Name}.";
                    _logger.LogError(error + " Skipping...", courseRow);
                    errors.Add(error);
                    continue;
                }

                var subject = subjects.FirstOrDefault(s => s.Code == courseRow.SubjectCode);
                if (subject == null)
                {
                    var error = $"Could not find subject with code {courseRow.SubjectCode} for {courseRow.Name}.";
                    _logger.LogError(error + " Skipping...", courseRow);
                    errors.Add(error);
                    continue;
                }

                if (courseRow.ScheduleTypes.Any(st => !scheduleTypesNames.Contains(st)))
                {
                    var missingScheduleTypes = string.Join(", ", courseRow.ScheduleTypes.Except(scheduleTypesNames));
                    var error = $"Could not find following schedule types for {courseRow.Name}: {missingScheduleTypes}.";
                    _logger.LogError(error + " Skipping...", courseRow);
                    errors.Add(error);
                    continue;
                }

                if (courseRow.CourseAttributes.Any(ca => !courseAttributesNames.Contains(ca)))
                {
                    var missingAttributes = string.Join(", ", courseRow.CourseAttributes.Except(courseAttributesNames));
                    var error = $"Could not find following attributes for {courseRow.Name}: {missingAttributes}.";
                    _logger.LogError(error + " Skipping...", courseRow);
                    errors.Add(error);
                    continue;
                }

                var course =
                    await _context.Courses
                        .Include(c => c.CourseScheduleTypes)
                        .Include(c => c.CourseCourseAttributes)
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
                        CourseCourseAttributes = new List<CourseCourseAttribute>(),
                        IsEnabled = true
                    };
                    _context.Courses.Add(course);
                }

                await _context.SaveChangesAsync();

                // Update schedule types
                _context.UpdateManyToMany(course.CourseScheduleTypes,
                    scheduleTypes
                        .Where(st => courseRow.ScheduleTypes.Contains(st.Name))
                        .Select(st => new CourseScheduleType
                        {
                            CourseId = course.Id,
                            ScheduleTypeId = st.Id
                        }),
                    cst => cst.ScheduleTypeId);

                // Update course attributes
                _context.UpdateManyToMany(course.CourseCourseAttributes,
                    courseAttributes
                        .Where(ca => courseRow.CourseAttributes.Contains(ca.Name))
                        .Select(ca => new CourseCourseAttribute
                        {
                            CourseId = course.Id,
                            CourseAttributeId = ca.Id
                        }),
                    cat => cat.CourseAttributeId);

                await _context.SaveChangesAsync();
            }

            if (errors.Count > 0)
            {
                _logger.LogError("Errors:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
            }
        }

        private class CourseRow
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