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
        public IList<CourseSection> CourseSections { get;set; }

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
    }
}
