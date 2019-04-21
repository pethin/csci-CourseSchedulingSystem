using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseAttributes
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public CourseAttribute CourseAttribute { get; set; }

        public bool InUse => CourseAttribute.CourseCourseAttributes.Any();

        public async Task<IActionResult> OnGetAsync()
        {
            CourseAttribute = await _context.CourseAttributes
                .Include(ca => ca.CourseCourseAttributes)
                .ThenInclude(cca => cca.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (CourseAttribute == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CourseAttribute = await _context.CourseAttributes
                .Include(ca => ca.CourseCourseAttributes)
                .ThenInclude(cca => cca.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (CourseAttribute != null)
            {
                if (InUse)
                {
                    return RedirectToPage();
                }
                
                _context.CourseAttributes.Remove(CourseAttribute);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}