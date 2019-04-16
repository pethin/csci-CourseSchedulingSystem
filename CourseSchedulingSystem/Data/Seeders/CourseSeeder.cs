using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.SS.UserModel;

namespace CourseSchedulingSystem.Data.Seeders
{
    /// <summary>
    /// Seeds courses from Excel fixture.
    /// </summary>
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
            var fixturePath = Path.Combine(PathUtilities.GetResourceDirectory(), "Fixtures", "Courses.xlsx");
            var courseRows = ConvertExcelToList(fixturePath);
            await InsertCourseRows(courseRows);
        }

        /// <summary>
        /// Loads the courses Excel file from the filePath and converts it into a list of rows.
        /// </summary>
        /// <param name="filePath">The file path to the fixture.</param>
        /// <returns>A list of Excel rows.</returns>
        private List<RowInfo<CourseExcelRow>> ConvertExcelToList(string filePath)
        {
            List<RowInfo<CourseExcelRow>> courses;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file as a workbook
                var workbook = WorkbookFactory.Create(stream);

                var mapper = new Mapper(workbook);

                courses = mapper.Take<CourseExcelRow>(0).ToList();

                workbook.Close();
            }

            return courses;
        }

        /// <summary>
        /// Inserts the course rows into the database.
        /// </summary>
        /// <param name="courseRows">The rows to insert.</param>
        /// <returns></returns>
        private async Task InsertCourseRows(List<RowInfo<CourseExcelRow>> courseRows)
        {
            var errors = new List<string>();

            // Get existing departments
            // Key: Code, Value: Department
            ConcurrentDictionary<string, Department> departments =
                new ConcurrentDictionary<string, Department>(
                    await _context.Departments.ToDictionaryAsync(d => d.Code, d => d));

            // Get existing subjects
            // Key: Code, Value: Subject
            ConcurrentDictionary<string, Subject> subjects =
                new ConcurrentDictionary<string, Subject>(
                    await _context.Subjects.ToDictionaryAsync(s => s.Code, s => s));

            // Get existing schedule types
            // Key: NormalizedName, Value: ScheduleType
            ConcurrentDictionary<string, ScheduleType> scheduleTypes =
                new ConcurrentDictionary<string, ScheduleType>(
                    await _context.ScheduleTypes.ToDictionaryAsync(st => st.NormalizedName, st => st));

            // Get existing course attributes
            // Key: NormalizedName, Value: ScheduleType
            ConcurrentDictionary<string, CourseAttribute> courseAttributes =
                new ConcurrentDictionary<string, CourseAttribute>(
                    await _context.CourseAttributes.ToDictionaryAsync(ca => ca.NormalizedName, ca => ca));

            // Get existing courses
            // Key: (SubjectCode, CourseLevel), Value: Course
            ConcurrentDictionary<Tuple<string, string>, Course> courses =
                new ConcurrentDictionary<Tuple<string, string>, Course>(
                    await _context.Courses
                        .Include(c => c.Subject)
                        .Include(c => c.CourseScheduleTypes)
                        .Include(c => c.CourseCourseAttributes)
                        .ToDictionaryAsync(c => Tuple.Create(c.Subject.Code, c.Number), c => c));

            foreach (var courseRow in courseRows)
            {
                if (!string.IsNullOrEmpty(courseRow.ErrorMessage))
                {
                    _logger.LogError(courseRow.ErrorMessage + " Skipping...", courseRow);
                    errors.Add(courseRow.ErrorMessage);
                    continue;
                }


                // Get department
                departments.TryGetValue(courseRow.Value.DepartmentCode, out Department department);
                if (department == null)
                {
                    var error =
                        $"Could not find department with code {courseRow.Value.DepartmentCode} for {courseRow.Value.Name}.";
                    _logger.LogError(error + " Skipping...", courseRow);
                    errors.Add(error);
                    continue;
                }

                // Get subject
                subjects.TryGetValue(courseRow.Value.SubjectCode, out Subject subject);
                if (subject == null)
                {
                    var error =
                        $"Could not find subject with code {courseRow.Value.SubjectCode} for {courseRow.Value.Name}.";
                    _logger.LogError(error + " Skipping...", courseRow);
                    errors.Add(error);
                    continue;
                }

                // Get schedule types
                var rowScheduleTypes = scheduleTypes
                    .Where(st => courseRow.Value.NormalizedScheduleTypes.Contains(st.Key))
                    .Select(st => st.Value)
                    .ToList();

                if (rowScheduleTypes.Count < courseRow.Value.NormalizedScheduleTypes.Count)
                {
                    var missingScheduleTypes = string.Join(", ",
                        courseRow.Value.NormalizedScheduleTypes.Except(
                            rowScheduleTypes.Select(rst => rst.NormalizedName)));

                    var error =
                        $"Could not find following schedule types for {courseRow.Value.Name}: {missingScheduleTypes}.";

                    _logger.LogError(error + " Skipping...", courseRow);
                    errors.Add(error);
                    continue;
                }

                // Get course attributes
                var rowCourseAttributes = courseAttributes
                    .Where(st => courseRow.Value.NormalizedCourseAttributes.Contains(st.Key))
                    .Select(st => st.Value)
                    .ToList();

                if (rowCourseAttributes.Count < courseRow.Value.NormalizedCourseAttributes.Count)
                {
                    var missingCourseAttributes = string.Join(", ",
                        courseRow.Value.NormalizedCourseAttributes.Except(
                            rowCourseAttributes.Select(rst => rst.NormalizedName)));

                    var error =
                        $"Could not find following course attributes for {courseRow.Value.Name}: {missingCourseAttributes}.";

                    _logger.LogError(error + " Skipping...", courseRow);
                    errors.Add(error);
                    continue;
                }

                // Create course if it doesn't exist
                courses.TryGetValue(Tuple.Create(courseRow.Value.SubjectCode, courseRow.Value.Number),
                    out Course course);

                if (course == null)
                {
                    course = new Course
                    {
                        DepartmentId = department.Id,
                        SubjectId = subject.Id,
                        Number = courseRow.Value.Number,
                        Title = courseRow.Value.Name,
                        CreditHours = courseRow.Value.CreditHours,
                        IsGraduate = courseRow.Value.IsGraduate,
                        IsUndergraduate = courseRow.Value.IsUndergraduate,
                        IsEnabled = true
                    };

                    // If the course was added, add it to the DB
                    if (courses.TryAdd(Tuple.Create(subject.Code, course.Number), course))
                    {
                        _context.Courses.Add(course);

                        // Add schedule types
                        _context.CourseScheduleTypes.AddRange(rowScheduleTypes
                            .Select(st => new CourseScheduleType
                            {
                                CourseId = course.Id,
                                ScheduleTypeId = st.Id
                            }));

                        // Add course attributes
                        _context.CourseCourseAttributes.AddRange(rowCourseAttributes
                            .Select(ca => new CourseCourseAttribute
                            {
                                CourseId = course.Id,
                                CourseAttributeId = ca.Id
                            }));
                    }
                }
                // If the course already created update values
                else
                {
                    course.DepartmentId = department.Id;
                    course.Title = courseRow.Value.Name;
                    course.CreditHours = courseRow.Value.CreditHours;
                    course.IsGraduate = courseRow.Value.IsGraduate;
                    course.IsUndergraduate = courseRow.Value.IsUndergraduate;

                    // Update schedule types
                    _context.UpdateManyToMany(course.CourseScheduleTypes,
                        rowScheduleTypes
                            .Select(st => new CourseScheduleType
                            {
                                CourseId = course.Id,
                                ScheduleTypeId = st.Id
                            }),
                        cst => cst.ScheduleTypeId);

                    // Update course attributes
                    _context.UpdateManyToMany(course.CourseCourseAttributes,
                        rowCourseAttributes
                            .Select(ca => new CourseCourseAttribute
                            {
                                CourseId = course.Id,
                                CourseAttributeId = ca.Id
                            }),
                        cat => cat.CourseAttributeId);
                }
            }

            // If any errors occured do not commit changes
            if (errors.Count > 0)
            {
                _logger.LogError("Errors:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
            }
            else
            {
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Mapping class for Excel spreadsheet.
        /// </summary>
        private class CourseExcelRow
        {
            private string _departmentCode;
            private string _subjectCode;
            private string _number;
            private string _name;
            private ICollection<string> _scheduleTypes;
            private ICollection<string> _courseAttributes;

            [Column("Department")]
            public string DepartmentCode
            {
                get => _departmentCode;
                set => _departmentCode = value?.Trim().ToUpperInvariant();
            }

            [Column("Subject")]
            public string SubjectCode
            {
                get => _subjectCode;
                set => _subjectCode = value?.Trim().ToUpperInvariant();
            }

            [Column("Number")]
            public string Number
            {
                get => _number;
                set => _number = value?.Trim();
            }

            [Column("Name")]
            public string Name
            {
                get => _name;
                set => _name = value?.Trim();
            }

            [Column("Credit Hours")] public decimal CreditHours { get; set; }


            [Column("Levels")]
            public string Levels
            {
                get => string.Join(
                    ", ",
                    new[] {IsUndergraduate ? "Undergraduate" : null, IsGraduate ? "Graduate" : null}
                        .Where(s => !string.IsNullOrWhiteSpace(s)));
                set
                {
                    var courseLevels = value.Split(",").Select(t => t.Trim()).ToHashSet();
                    IsGraduate = courseLevels.Contains("Graduate");
                    IsUndergraduate = courseLevels.Contains("Undergraduate");
                }
            }

            [Column("Schedule Types")]
            public string ScheduleTypesColumn
            {
                get => string.Join(", ", _scheduleTypes);
                set
                {
                    _scheduleTypes = value?.Split(",")
                                         .Select(st => st.Trim())
                                         .Where(st => !string.IsNullOrEmpty(st))
                                         .ToList() ?? new List<string>();

                    NormalizedScheduleTypes = _scheduleTypes
                        .Select(st => st.ToUpperInvariant())
                        .ToHashSet();
                }
            }


            [Column("Course Attributes")]
            public string CourseAttributesColumn
            {
                get => string.Join(", ", _courseAttributes);
                set
                {
                    _courseAttributes = value?
                                            .Split(",")
                                            .Select(st => st.Trim())
                                            .Where(st => !string.IsNullOrEmpty(st))
                                            .ToList() ?? new List<string>();

                    NormalizedCourseAttributes = _courseAttributes
                        .Select(st => st.ToUpperInvariant())
                        .ToHashSet();
                }
            }

            [Ignore] public bool IsGraduate { get; set; }

            [Ignore] public bool IsUndergraduate { get; set; }

            [Ignore] public ICollection<string> NormalizedScheduleTypes { get; set; }

            [Ignore] public ICollection<string> NormalizedCourseAttributes { get; set; }
        }
    }
}