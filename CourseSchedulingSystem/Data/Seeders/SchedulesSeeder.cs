using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CourseSchedulingSystem.Data.Seeders
{
    /// <summary>
    /// Seeds 2018 Summer, 2018 Fall, and 2019 Spring course sections. 
    /// </summary>
    public class SchedulesSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SchedulesSeeder> _logger;

        public SchedulesSeeder(ApplicationDbContext context, ILogger<SchedulesSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        private InstructionalMethod InstructionalMethodClass { get; set; }
        private InstructionalMethod InstructionalMethodWeb { get; set; }

        private ScheduleType ScheduleTypeLecture { get; set; }

        public async Task SeedAsync()
        {
            InstructionalMethodClass = await _context.InstructionalMethods
                .FirstOrDefaultAsync(im => im.Code == "CLASS");
            InstructionalMethodWeb = await _context.InstructionalMethods
                .FirstOrDefaultAsync(im => im.Code == "WEB");

            if (InstructionalMethodClass == null)
            {
                _logger.LogError("Could not find instructional method CLASS");
                return;
            }

            if (InstructionalMethodWeb == null)
            {
                _logger.LogError("Could not find instructional method WEB");
                return;
            }

            ScheduleTypeLecture = await _context.ScheduleTypes.FirstOrDefaultAsync(im => im.Code == "LEC");

            if (ScheduleTypeLecture == null)
            {
                _logger.LogError("Could not find schedule type LEC");
                return;
            }

            var termsDirectory = Path.Combine(PathUtilities.GetResourceDirectory(), "Fixtures", "Terms");
            foreach (var termFile in Directory.EnumerateFiles(termsDirectory, "*.xlsx"))
            {
                var term = Path.GetFileNameWithoutExtension(termFile).Trim();
                var dataTable = LoadExcelFixture(termFile);
                var courseRows = ConvertDataTableToList(dataTable);
                await InsertCourseRowsAsync(term, courseRows);
            }
        }

        private DataTable LoadExcelFixture(string filePath)
        {
            var dt = new DataTable();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var wb = new XSSFWorkbook(stream); // This will read 2007 Excel format

                // Evaluate formulas
                XSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);

                var sheet = wb.GetSheetAt(0);
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

                wb.Close();
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
                    .Select(name =>
                    {
                        var parts = name.Split(' ');
                        var first = parts[0];
                        string middle = null, last = "";

                        if (parts.Length > 2)
                        {
                            middle = string.Join(' ', parts.Skip(1).Take(parts.Length - 2));
                        }

                        if (parts.Length > 1)
                            last = parts.Last();

                        return new InstructorDTO
                        {
                            First = first,
                            Middle = middle,
                            Last = last
                        };
                    }).ToList();

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
                    Section = Convert.ToInt32(dr["Section"]),

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

        private async Task InsertCourseRowsAsync(string termName, IList<CourseSectionRow> rows)
        {
            var errors = new List<string>();

            // Find or create term
            var term = await _context.Terms
                .FirstOrDefaultAsync(t => t.NormalizedName == termName.ToUpperInvariant());

            if (term == null)
            {
                term = new Term(termName);
                _context.Terms.Add(term);
            }

            // Get existing courses
            // Key: (SubjectCode, CourseLevel), Value: Course
            ConcurrentDictionary<Tuple<string, string>, Course> courses =
                new ConcurrentDictionary<Tuple<string, string>, Course>(
                    await _context.Courses
                        .Include(c => c.Subject)
                        .ToDictionaryAsync(c => Tuple.Create(c.Subject.Code, c.Number), c => c));

            // Get existing term parts
            // Key: Normalized Name, Value: TermPart
            ConcurrentDictionary<string, TermPart> termParts = new ConcurrentDictionary<string, TermPart>(
                await _context.TermParts
                    .Where(tp => tp.TermId == term.Id)
                    .ToDictionaryAsync(tp => tp.NormalizedName, tp => tp));

            // Get existing buildings
            // Key: Code, Value: Building
            ConcurrentDictionary<string, Building> buildings = new ConcurrentDictionary<string, Building>(
                await _context.Buildings.ToDictionaryAsync(tp => tp.Code, tp => tp));

            // Get existing rooms
            // Key: (BuildingId, Number), Value: Room
            ConcurrentDictionary<Tuple<Guid, string>, Room> rooms = new ConcurrentDictionary<Tuple<Guid, string>, Room>(
                await _context.Rooms.ToDictionaryAsync(r => Tuple.Create(r.BuildingId, r.Number), r => r));

            // Get course sections
            // Key: (CourseId, SectionNumber), Value: Room
            ConcurrentDictionary<Tuple<Guid, int>, CourseSection> courseSections =
                new ConcurrentDictionary<Tuple<Guid, int>, CourseSection>(
                    await _context.CourseSections
                        .Include(cs => cs.TermPart)
                        .Include(cs => cs.ScheduledMeetingTimes)
                        .Where(cs => cs.TermPart.TermId == term.Id)
                        .ToDictionaryAsync(cs => Tuple.Create(cs.CourseId, cs.Section), cs => cs));

            // Get existing instructors
            // Key: NormalizedName, Value: Room
            ConcurrentDictionary<string, Instructor> instructors =
                new ConcurrentDictionary<string, Instructor>(
                    await _context.Instructors.ToDictionaryAsync(i => i.NormalizedName, i => i));

            foreach (var row in rows)
            {
                // Try to find course
                courses.TryGetValue(Tuple.Create(row.SubjectCode, row.CourseNumber), out Course course);

                if (course == null)
                {
                    errors.Add($"Could not find course {row.SubjectCode}{row.CourseNumber} for section {row.Section}");
                    continue;
                }

                // Get or create part of term
                termParts.TryGetValue(row.PartOfTerm, out TermPart partOfTerm);

                if (partOfTerm == null)
                {
                    partOfTerm = new TermPart
                    {
                        TermId = term.Id,
                        Name = row.PartOfTerm,
                        StartDate = row.StartDate,
                        EndDate = row.EndDate
                    };

                    // If the part of term was added, add it to the DB
                    if (termParts.TryAdd(partOfTerm.NormalizedName, partOfTerm))
                    {
                        _context.TermParts.Add(partOfTerm);
                    }
                    else
                    {
                        partOfTerm = termParts[partOfTerm.NormalizedName];
                    }
                }

                // Get/create building and room if needed
                Building building = null;
                Room room = null;

                if (!string.IsNullOrEmpty(row.BuildingCode))
                {
                    // Get or create building
                    buildings.TryGetValue(row.BuildingCode, out building);

                    if (building == null)
                    {
                        building = new Building
                        {
                            Code = row.BuildingCode,
                            Name = row.BuildingCode,
                            IsEnabled = true
                        };

                        // If the building was added, add it to the DB
                        if (buildings.TryAdd(building.Code, building))
                        {
                            _context.Buildings.Add(building);
                        }
                        else
                        {
                            building = buildings[building.Code];
                        }
                    }

                    // Get or create room
                    rooms.TryGetValue(Tuple.Create(building.Id, row.Room), out room);

                    if (room == null)
                    {
                        room = new Room
                        {
                            BuildingId = building.Id,
                            Number = row.Room,
                            Capacity = row.Capacity,
                            IsEnabled = true
                        };

                        // If the room was added, add it to the DB
                        if (rooms.TryAdd(Tuple.Create(building.Id, room.Number), room))
                        {
                            _context.Rooms.Add(room);
                        }
                        else
                        {
                            room = rooms[Tuple.Create(building.Id, room.Number)];

                            if (room.Capacity < row.Capacity)
                            {
                                room.Capacity = row.Capacity;
                            }
                        }
                    }
                    else if (room.Capacity < row.Capacity)
                    {
                        room.Capacity = row.Capacity;
                    }
                }

                // Get or create instructor
                var sectionInstructors = new List<Instructor>();

                foreach (var instructorDto in row.Instructors)
                {
                    // Get or create instructor
                    instructors.TryGetValue(instructorDto.NormalizedName, out Instructor instructor);

                    if (instructor == null)
                    {
                        instructor = new Instructor
                        {
                            FirstName = instructorDto.First,
                            Middle = instructorDto.Middle,
                            LastName = instructorDto.Last,
                            IsActive = true
                        };

                        // If the part of term was added, add it to the DB
                        if (instructors.TryAdd(instructor.NormalizedName, instructor))
                        {
                            _context.Instructors.Add(instructor);
                        }
                        else
                        {
                            instructor = instructors[instructor.NormalizedName];
                        }
                    }

                    sectionInstructors.Add(instructor);
                }

                // Create course section if it doesn't exist
                courseSections.TryGetValue(Tuple.Create(course.Id, row.Section), out CourseSection courseSection);

                if (courseSection == null)
                {
                    var isOnline = row.BuildingCode == "INTR";

                    courseSection = new CourseSection
                    {
                        CourseId = course.Id,
                        Section = row.Section,
                        TermPartId = partOfTerm.Id,
                        InstructionalMethodId =
                            isOnline
                                ? InstructionalMethodWeb.Id
                                : InstructionalMethodClass.Id, // Either class or online
                        ScheduleTypeId = ScheduleTypeLecture.Id, // Default to lecture
                        MaximumCapacity = row.Capacity,
                        ScheduledMeetingTimes = new List<ScheduledMeetingTime>()
                    };

                    // If the room was added, add it to the DB
                    if (courseSections.TryAdd(Tuple.Create(courseSection.CourseId, courseSection.Section),
                        courseSection))
                    {
                        _context.CourseSections.Add(courseSection);

                        // Create the scheduled meeting time
                        var scheduledMeetingTime = new ScheduledMeetingTime
                        {
                            CourseSectionId = courseSection.Id,
                            MeetingTypeId = MeetingType.ClassMeetingType.Id,
                            StartTime = row.StartTime,
                            EndTime = row.EndTime,
                            Monday = row.Monday,
                            Tuesday = row.Tuesday,
                            Wednesday = row.Wednesday,
                            Thursday = row.Thursday,
                            Friday = row.Friday,
                            Saturday = row.Saturday,
                            Sunday = row.Sunday,
                        };
                        _context.ScheduledMeetingTimes.Add(scheduledMeetingTime);

                        // Add instructors to scheduled meeting time
                        var scheduledMeetingTimeInstructors = sectionInstructors.Select(i =>
                            new ScheduledMeetingTimeInstructor
                            {
                                ScheduledMeetingTimeId = scheduledMeetingTime.Id,
                                InstructorId = i.Id
                            });
                        _context.ScheduledMeetingTimeInstructors.AddRange(scheduledMeetingTimeInstructors);

                        // Add room to scheduled meeting time
                        // TODO: Allow multiple rooms
                        if (room != null)
                        {
                            _context.ScheduledMeetingTimeRooms.Add(new ScheduledMeetingTimeRoom
                            {
                                ScheduledMeetingTimeId = scheduledMeetingTime.Id,
                                RoomId = room.Id
                            });
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                _logger.LogError($"There were errors adding {termName}. Course sections were not added.");
                _logger.LogError("Errors:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
            }
            else
            {
                await _context.SaveChangesAsync();
            }
        }

        private class CourseSectionRow
        {
            public string SubjectCode { get; set; }
            public string CourseNumber { get; set; }
            public int Section { get; set; }

            public string PartOfTerm { get; set; }

            public bool Monday { get; set; }
            public bool Tuesday { get; set; }
            public bool Wednesday { get; set; }
            public bool Thursday { get; set; }
            public bool Friday { get; set; }
            public bool Saturday { get; set; }
            public bool Sunday { get; set; }

            public int Capacity { get; set; }

            public IList<InstructorDTO> Instructors { get; set; }

            public TimeSpan? StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            public string BuildingCode { get; set; }
            public string Room { get; set; }
        }

        private class InstructorDTO
        {
            public string First { get; set; }
            public string Middle { get; set; }
            public string Last { get; set; }

            public string NormalizedName =>
                string.Join(" ",
                        new List<String> {First, Middle, Last}
                            .Where(part => !string.IsNullOrWhiteSpace(part)))
                    .ToUpperInvariant();
        }
    }
}