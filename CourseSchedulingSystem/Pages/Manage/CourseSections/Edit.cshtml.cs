using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class EditModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CourseSection CourseSection { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CourseSection = await _context.CourseSections
                .Include(c => c.Course)
                .Include(c => c.InstructionalMethod)
                .Include(c => c.ScheduleType)
                .Include(c => c.TermPart).FirstOrDefaultAsync(m => m.Id == id);

            if (CourseSection == null)
            {
                return NotFound();
            }
           ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Number");
           ViewData["InstructionalMethodId"] = new SelectList(_context.InstructionalMethods, "Id", "Code");
           ViewData["ScheduleTypeId"] = new SelectList(_context.ScheduleTypes, "Id", "Code");
           ViewData["TermPartId"] = new SelectList(_context.TermParts, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CourseSection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseSectionExists(CourseSection.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CourseSectionExists(Guid id)
        {
            return _context.CourseSections.Any(e => e.Id == id);
        }
    }
}
