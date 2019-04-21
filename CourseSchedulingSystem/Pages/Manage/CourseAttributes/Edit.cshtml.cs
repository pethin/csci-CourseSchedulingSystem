using System;
using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseAttributes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public CourseAttribute CourseAttribute { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CourseAttribute = await _context.CourseAttributes.FirstOrDefaultAsync(m => m.Id == Id);

            if (CourseAttribute == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var courseAttribute = await _context.CourseAttributes.FindAsync(Id);

            if (await TryUpdateModelAsync(
                courseAttribute,
                "CourseAttribute",
                at => at.Name))
            {
                await courseAttribute.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}