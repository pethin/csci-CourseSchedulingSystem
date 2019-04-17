using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Subjects
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public Subject Subject { get; set; }

        public bool InUse => Subject.Courses.Any();

        public async Task<IActionResult> OnGetAsync()
        {
            Subject = await _context.Subjects
                .Include(s => s.Courses)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Subject == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Subject = await _context.Subjects
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Subject != null)
            {
                if (InUse)
                {
                    return RedirectToPage();
                }

                _context.Subjects.Remove(Subject);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}