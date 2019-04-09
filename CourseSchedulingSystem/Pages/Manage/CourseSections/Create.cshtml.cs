using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class CreateModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public CreateModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Number");
        ViewData["InstructionalMethodId"] = new SelectList(_context.InstructionalMethods, "Id", "Code");
        ViewData["ScheduleTypeId"] = new SelectList(_context.ScheduleTypes, "Id", "Code");
        ViewData["TermPartId"] = new SelectList(_context.TermParts, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public CourseSection CourseSection { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.CourseSections.Add(CourseSection);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}