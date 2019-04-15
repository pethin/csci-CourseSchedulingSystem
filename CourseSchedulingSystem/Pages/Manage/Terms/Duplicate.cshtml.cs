using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class DuplicateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DuplicateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Term Term { get; set; }

        public string SourceTermName { get; set; }
        public IEnumerable<TermPart> TermParts { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Term = await _context.Terms.FirstOrDefaultAsync(m => m.Id == id);

            if (Term == null) return NotFound();

            SourceTermName = Term.Name;
            TermParts = _context.TermParts.Where(tp => tp.TermId == Term.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var term = await _context.Terms
                .Include(t => t.TermParts)
                .ThenInclude(tp => tp.CourseSections)
                .ThenInclude(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.ScheduledMeetingTimeInstructors)
                .Include(t => t.TermParts)
                .ThenInclude(tp => tp.CourseSections)
                .ThenInclude(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.ScheduledMeetingTimeRooms)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (term == null)
            {
                return NotFound();
            }

            SourceTermName = term.Name;
            
            // Clone TermId
            var termId = term.Id;
            TermParts = _context.TermParts.Where(tp => tp.TermId == termId);

            term.Id = Guid.NewGuid();
            term.Name = Term.Name;

            await term.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

            if (!ModelState.IsValid) return Page();

            term.TermParts.ForEach(part =>
            {
                part.Id = Guid.NewGuid();
                part.TermId = Term.Id;
                part.CourseSections.ForEach(courseSection =>
                {
                    courseSection.Id = Guid.NewGuid();
                    courseSection.TermPartId = part.Id;
                    courseSection.ScheduledMeetingTimes.ForEach(smt =>
                    {
                        smt.Id = Guid.NewGuid();
                        smt.CourseSectionId = courseSection.Id;
                        smt.ScheduledMeetingTimeInstructors.ForEach(smti => smti.ScheduledMeetingTimeId = smt.Id);
                        smt.ScheduledMeetingTimeRooms.ForEach(smtr => smtr.ScheduledMeetingTimeId = smt.Id);
                    });
                });
            });

            _context.Terms.Add(term);

            await _context.SaveChangesAsync();

            return RedirectToPage("Edit", new {id = term.Id});
        }
    }
}