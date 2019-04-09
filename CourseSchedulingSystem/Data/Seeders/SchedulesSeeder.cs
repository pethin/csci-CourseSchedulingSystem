using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    public class SchedulesSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SchedulesSeeder> _logger;

        public SchedulesSeeder(ApplicationDbContext context, ILogger<SchedulesSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            var termsDirectory = Path.Combine(PathUtilities.GetResourceDirectory(), "Fixtures", "Terms");
            foreach (var termFile in Directory.EnumerateFiles(termsDirectory, "*.xlsx"))
            {
                var dataTable = LoadExcelFixture(termFile);
                var courseRows = ConvertDataTableToList(dataTable);
                await InsertCourseRows(courseRows);
            }
        }

        private DataTable LoadExcelFixture(string filePath)
        {
            var dt = new DataTable();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
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

        private IList<CourseSectionRow> ConvertDataTableToList(DataTable dt)
        {
            IList<CourseSectionRow> courseSections = new List<CourseSectionRow>();

            foreach (DataRow dr in dt.Rows)
            {
                var days = dr["Days"].ToString().Trim().ToUpperInvariant();
                var instructors = dr["Instructors"].ToString().Split(',')
                    .Select(s =>
                    {
                        // Remove (P)
                        int charLocation = s.IndexOf('(', StringComparison.Ordinal);

                        if (charLocation > 0)
                        {
                            return s.Substring(0, charLocation).Trim();
                        }

                        return s.Trim();
                    })
                    .ToList();

                var time = dr["Time"].ToString()?.Trim();
                TimeSpan? startTime = null, endTime = null;
                if (!string.IsNullOrWhiteSpace(time) && time.ToUpperInvariant() != "TBA")
                {
                    var parts = time.Split('-');

                    DateTime.TryParseExact(
                        parts[0], 
                        @"h:mm tt",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.NoCurrentDateDefault,
                        out var output);
                    startTime = output.Subtract(output.Date);
                    
                    DateTime.TryParseExact(
                        parts[0], 
                        @"h:mm tt",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.NoCurrentDateDefault,
                        out output);
                    endTime = output.Subtract(output.Date);
                }

                var location = dr["Location"].ToString()?.Trim();
                string buildingCode = null, room = null;
                if (!string.IsNullOrWhiteSpace(location) && location.ToUpperInvariant() != "TBA")
                {
                    var parts = location.Split(" ")
                        .Where(part => !string.IsNullOrWhiteSpace(part))
                        .ToArray();

                    buildingCode = parts[0].Trim().ToUpperInvariant();
                    room = parts[1].Trim().ToUpperInvariant();
                }

                var courseSection = new CourseSectionRow
                {
                    PartOfTerm = dr["Part of Term"].ToString().Trim(),
                    SubjectCode = dr["Subject"].ToString().Trim().ToUpperInvariant(),
                    CourseNumber = dr["Course"].ToString().Trim().ToUpperInvariant(),

                    Monday = days.Contains('M'),
                    Tuesday = days.Contains('T'),
                    Wednesday = days.Contains('W'),
                    Thursday = days.Contains('R'),
                    Friday = days.Contains('F'),
                    Saturday = days.Contains('S'),
                    Sunday = days.Contains('U'),

                    Capacity = Convert.ToInt32(dr["Cap"]),

                    Instructors = instructors,

                    StartTime = startTime,
                    EndTime = endTime,
                    
                    StartDate = DateTime.Parse(dr["Start Date"].ToString()),
                    EndDate = DateTime.Parse(dr["End Date"].ToString()),

                    BuildingCode = buildingCode,
                    Room = room
                };

                courseSections.Add(courseSection);
            }

            return courseSections;
        }

        private async Task InsertCourseRows(IList<CourseSectionRow> courseSections)
        {
            var errors = new List<string>();

            // TODO: DB logic
            await Task.Yield();

            if (errors.Count > 0)
            {
                _logger.LogError("Errors:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
            }
        }

        private class CourseSectionRow
        {
            public string PartOfTerm { get; set; }
            public string SubjectCode { get; set; }
            public string CourseNumber { get; set; }

            public bool Monday { get; set; }
            public bool Tuesday { get; set; }
            public bool Wednesday { get; set; }
            public bool Thursday { get; set; }
            public bool Friday { get; set; }
            public bool Saturday { get; set; }
            public bool Sunday { get; set; }

            public int Capacity { get; set; }

            public IList<string> Instructors { get; set; }

            public TimeSpan? StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            public string BuildingCode { get; set; }
            public string Room { get; set; }
        }
    }
}