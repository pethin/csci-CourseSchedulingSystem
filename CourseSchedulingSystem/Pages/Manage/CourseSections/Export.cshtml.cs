using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npoi.Mapper;
using Npoi.Mapper.Attributes;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class ExportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ExportModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [FromRoute] public Guid TermId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var term = await _context.Terms.FindAsync(TermId);

            if (term == null) return NotFound();

            // Query DB
            var courseSections = await _context.CourseSections
                .Where(cs => cs.TermPart.TermId == TermId)
                // Part of Term
                .Include(cs => cs.TermPart)
                // Course
                .Include(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                // Department
                .Include(cs => cs.Course)
                .ThenInclude(c => c.Department)
                // Instructional Method
                .Include(cs => cs.InstructionalMethod)
                // Schedule Type
                .Include(cs => cs.ScheduleType)
                // MeetingType
                .Include(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.MeetingType)
                // Instructors
                .Include(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.ScheduledMeetingTimeInstructors)
                .ThenInclude(smti => smti.Instructor)
                // Rooms
                .Include(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.ScheduledMeetingTimeRooms)
                .ThenInclude(smtr => smtr.Room)
                .ThenInclude(r => r.Building)
                .ToListAsync();

            // Flatten scheduled meeting times
            var rows = courseSections.SelectMany(section => section.ScheduledMeetingTimes.Any()
                ? section.ScheduledMeetingTimes.Select(smt => new RowModel
                {
                    Course = section.Course.Subject.Code + section.Course.Number,
                    Section = section.Section,
                    CreditHours = section.Course.CreditHours,
                    CourseTitle = section.Course.Title,
                    MeetingType = smt.MeetingType.Code,
                    Instructor = smt.ScheduledMeetingTimeInstructors.Any()
                        ? string.Join(", ",
                            smt.ScheduledMeetingTimeInstructors.Select(smti => smti.Instructor.FullName))
                        : "TBA",
                    InstructionalMethod = section.InstructionalMethod.Code,
                    ScheduleType = section.ScheduleType.Code,
                    Department = section.Course.Department.Code,
                    Location = smt.ScheduledMeetingTimeRooms.Any()
                        ? string.Join(", ", smt.ScheduledMeetingTimeRooms.Select(smtr => smtr.Room.Identifier))
                        : "TBA",
                    Time = smt.StartTime != null && smt.EndTime != null
                        ? smt.StartTimeText + " - " + smt.EndTimeText
                        : "TBA",
                    Days = smt.DaysOfWeek,
                    Max = section.MaximumCapacity,
                    RoomCapacity = smt.ScheduledMeetingTimeRooms.Any()
                        ? smt.ScheduledMeetingTimeRooms.Sum(smtr => smtr.Room.Capacity)
                        : 0,
                    PartOfTerm = section.TermPart.Name,
                    SessionStart = section.TermPart.StartDate,
                    SessionEnd = section.TermPart.EndDate
                })
                : new[]
                {
                    new RowModel
                    {
                        Course = section.Course.Subject.Code + section.Course.Number,
                        Section = section.Section,
                        CreditHours = section.Course.CreditHours,
                        CourseTitle = section.Course.Title,
                        MeetingType = "TBA",
                        Instructor = "TBA",
                        InstructionalMethod = section.InstructionalMethod.Code,
                        ScheduleType = section.ScheduleType.Code,
                        Department = section.Course.Department.Code,
                        Location = "TBA",
                        Time = "TBA",
                        Days = "TBA",
                        Max = section.MaximumCapacity,
                        RoomCapacity = 0,
                        PartOfTerm = section.TermPart.Name,
                        SessionStart = section.TermPart.StartDate,
                        SessionEnd = section.TermPart.EndDate
                    }
                });

            // Serialize to Excel
            var ms = new NpoiMemoryStream();
            
            // Disable closing of stream
            ms.AllowClose = false;
            
            var mapper = new Mapper();
            mapper.Save(ms, rows);
            await ms.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);

            // Auto-sizing does not work
//            // Auto-size columns
//            var workbook = WorkbookFactory.Create(ms);
//            var sheet = workbook.GetSheetAt(0);
//            var headerRow = sheet.GetRow(0);
//
//            for (short i = 0, numRows = headerRow.LastCellNum; i < numRows; i++)
//            {
//                sheet.AutoSizeColumn(i);
//            }
//
//            workbook.Close();
//            await ms.FlushAsync();
//            ms.Seek(0, SeekOrigin.Begin);
            
            // Re-enable closing of stream 
            ms.AllowClose = true;
            
            return File(ms, "application/ms-excel", $"{term.Name}.xlsx");
        }

        class RowModel
        {
            [Column("Course")] public string Course { get; set; }

            [Column("Section", CustomFormat = "000")]
            public int Section { get; set; }

            [Column("Credit Hours", CustomFormat = "0.000")]
            public decimal CreditHours { get; set; }

            [Column("Course Title")] public string CourseTitle { get; set; }

            [Column("Meeting Type")] public string MeetingType { get; set; }

            [Column("Instructor")] public string Instructor { get; set; }

            [Column("Instructional Method")] public string InstructionalMethod { get; set; }

            [Column("Schedule Type")] public string ScheduleType { get; set; }

            [Column("Dept")] public string Department { get; set; }

            [Column("Location")] public string Location { get; set; }

            [Column("Time")] public string Time { get; set; }

            [Column("Days")] public string Days { get; set; }

            [Column("Max")] public int Max { get; set; }

            [Column("Room Capacity")] public int RoomCapacity { get; set; }

            [Column("Part Of Term")] public string PartOfTerm { get; set; }

            [Column("Session Start")] public DateTime? SessionStart { get; set; }

            [Column("Session End")] public DateTime? SessionEnd { get; set; }

            [Column("FootNotes")] public string FootNotes { get; set; }
        }
        
        public class NpoiMemoryStream : MemoryStream
        {
            public NpoiMemoryStream()
            {
                // We always want to close streams by default to
                // force the developer to make the conscious decision
                // to disable it.  Then, they're more apt to remember
                // to re-enable it.  The last thing you want is to
                // enable memory leaks by default.  ;-)
                AllowClose = true;
            }

            public bool AllowClose { get; set; }

            public override void Close()
            {
                if (AllowClose)
                    base.Close();
            }
        }
    }
}