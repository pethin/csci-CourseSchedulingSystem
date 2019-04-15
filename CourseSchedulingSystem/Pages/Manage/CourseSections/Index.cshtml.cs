using System;
using System.Collections.Generic;
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

        public Term Term { get; set; }
        public IList<CourseSection> CourseSections { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? termId)
        {
            if (termId == null)
            {
                return NotFound();
            }

            Term = await _context.Terms.Where(t => t.Id == termId).FirstOrDefaultAsync();

            if (Term == null)
            {
                return NotFound();
            }

            CourseSections = await _context.CourseSections
                .Include(c => c.TermPart)
                .Where(c => c.TermPart.TermId == Term.Id)
                .Include(c => c.Course)
                .ThenInclude(c => c.Subject)
                .Include(c => c.InstructionalMethod)
                .Include(c => c.ScheduleType)
                .OrderBy(c => c.Course.Subject.Code)
                .ThenBy(c => c.Course.Number)
                .ThenBy(c => c.Section)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDuplicateAsync(Guid? termId, Guid? sectionId)
        {
            if (termId == null)
            {
                return NotFound();
            }

            if (sectionId == null)
            {
                ModelState.AddModelError(string.Empty, "Could not duplicate section: missing ID.");
                return await OnGetAsync(termId);
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
                return await OnGetAsync(termId);
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

            return RedirectToAction("");
        }
    }
}