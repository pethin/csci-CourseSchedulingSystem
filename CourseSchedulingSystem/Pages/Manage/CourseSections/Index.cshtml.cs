using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid TermId { get; set; }

        public Term Term { get; set; }
        public IList<CourseViewModel> CourseSections { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Term = await _context.Terms.Where(t => t.Id == TermId).FirstOrDefaultAsync();

            if (Term == null)
            {
                return NotFound();
            }

            CourseSections = (await _context.CourseSections
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
                    .ToListAsync())
                .Select(section =>
                {
                    var classScheduledMeetingTime =
                        section.ScheduledMeetingTimes.FirstOrDefault(smt =>
                            smt.MeetingType.Id == MeetingType.ClassMeetingType.Id);

                    if (classScheduledMeetingTime == null)
                    {
                        return new CourseViewModel
                        {
                            Id = section.Id,
                            CourseId = section.CourseId,
                            CourseIdentifier = section.Course.Subject.Code + section.Course.Number,
                            Section = section.Section,
                            CreditHours = section.Course.CreditHours,
                            CourseTitle = section.Course.Title,
                            Instructor = "TBA",
                            InstructionalMethod = section.InstructionalMethod.Code,
                            ScheduleType = section.ScheduleType.Code,
                            Department = section.Course.Department.Code,
                            Location = "TBA",
                            Time = "TBA",
                            Days = "TBA",
                            Capacity = section.MaximumCapacity,
                            PartOfTerm = section.TermPart.Name,
                            SchedulingNotifications = section.SchedulingNotifications
                        };
                    }

                    return new CourseViewModel
                    {
                        Id = section.Id,
                        CourseId = section.CourseId,
                        CourseIdentifier = section.Course.Subject.Code + section.Course.Number,
                        Section = section.Section,
                        CreditHours = section.Course.CreditHours,
                        CourseTitle = section.Course.Title,
                        Instructor = classScheduledMeetingTime.InstructorsText,
                        InstructionalMethod = section.InstructionalMethod.Code,
                        ScheduleType = section.ScheduleType.Code,
                        Department = section.Course.Department.Code,
                        Location = classScheduledMeetingTime.RoomsText,
                        Time = classScheduledMeetingTime.StartTime != null && classScheduledMeetingTime.EndTime != null
                            ? classScheduledMeetingTime.StartTimeText + " - " + classScheduledMeetingTime.EndTimeText
                            : "TBA",
                        Days = classScheduledMeetingTime.DaysOfWeek,
                        Capacity = section.MaximumCapacity,
                        PartOfTerm = section.TermPart.Name,
                        SchedulingNotifications = section.SchedulingNotifications
                    };
                })
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostDuplicateAsync(Guid? sectionId)
        {
            if (sectionId == null)
            {
                ModelState.AddModelError(string.Empty, "Could not duplicate section: missing ID.");
                return await OnGetAsync();
            }

            var courseSection = await _context.CourseSections
                .Include(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.ScheduledMeetingTimeInstructors)
                .Include(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.ScheduledMeetingTimeRooms)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (courseSection == null)
            {
                ModelState.AddModelError(string.Empty,
                    $"Could not duplicate section: could not find section with ID {sectionId}.");
                return await OnGetAsync();
            }

            courseSection.Id = Guid.NewGuid();

            // Get next section number
            var term = await _context.Terms
                .Include(t => t.TermParts)
                .Where(t => t.TermParts.Any(tp => tp.Id == courseSection.TermPartId))
                .FirstOrDefaultAsync();

            int nextSectionNumber = await _context.CourseSections
                                        .Include(cs => cs.TermPart)
                                        .Where(cs => cs.TermPart.TermId == term.Id)
                                        .Where(cs => cs.CourseId == courseSection.CourseId)
                                        // Ignore contract courses and restricted courses
                                        .Where(cs =>
                                            (cs.Section < 80) || (cs.Section >= 90 && cs.Section < 600) ||
                                            (cs.Section >= 700))
                                        .OrderByDescending(cs => cs.Section)
                                        .Select(cs => cs.Section)
                                        .FirstOrDefaultAsync() + 1;

            // Update section number
            courseSection.Section = nextSectionNumber;

            courseSection.ScheduledMeetingTimes.ForEach(smt =>
            {
                smt.Id = Guid.NewGuid();
                smt.CourseSectionId = courseSection.Id;
                smt.ScheduledMeetingTimeInstructors.ForEach(smti => smti.ScheduledMeetingTimeId = smt.Id);
                smt.ScheduledMeetingTimeRooms.ForEach(smtr => smtr.ScheduledMeetingTimeId = smt.Id);
            });

            _context.CourseSections.Add(courseSection);
            await _context.SaveChangesAsync();

            // TODO: Recalculate SchedulingNotifications
            
            return RedirectToAction("");
        }
    }

    public class CourseViewModel
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }

        [Display(Name = "Course")] public string CourseIdentifier { get; set; }

        [DisplayFormat(DataFormatString = "{0:D3}", ApplyFormatInEditMode = true)]
        public int Section { get; set; }

        [Display(Name = "Credit Hours")]
        [DisplayFormat(DataFormatString = "{0:F3}", ApplyFormatInEditMode = true)]
        public decimal CreditHours { get; set; }

        [Display(Name = "Course Title")] public string CourseTitle { get; set; }

        [Display(Name = "Instructor")] public string Instructor { get; set; }

        [Display(Name = "Instructional Method")]
        public string InstructionalMethod { get; set; }

        [Display(Name = "Schedule Type")] public string ScheduleType { get; set; }

        [Display(Name = "Dept")] public string Department { get; set; }

        [Display(Name = "Location")] public string Location { get; set; }

        [Display(Name = "Time")] public string Time { get; set; }

        [Display(Name = "Days")] public string Days { get; set; }

        [Display(Name = "Capacity")] public int Capacity { get; set; }

        [Display(Name = "Part Of Term")] public string PartOfTerm { get; set; }

        [Display(Name = "Errors")] public SchedulingNotifications SchedulingNotifications { get; set; }
    }
}