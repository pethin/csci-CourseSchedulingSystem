using System;
using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseSchedulingSystem.Pages.Manage.Subjects
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public Subject Subject { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Subject = await _context.Subjects.FindAsync(Id);

            if (Subject == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var subject = await _context.Subjects.FindAsync(Id);

            if (await TryUpdateModelAsync(
                subject,
                "Subject",
                s => s.Name))
            {
                await subject.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}