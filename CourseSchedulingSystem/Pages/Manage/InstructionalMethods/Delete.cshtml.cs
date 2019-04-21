using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.InstructionalMethods
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [FromRoute] public Guid Id { get; set; }

        [BindProperty] public InstructionalMethod InstructionalMethod { get; set; }
        
        public bool InUse => InstructionalMethod.CourseSections.Any();

        public async Task<IActionResult> OnGetAsync()
        {
            InstructionalMethod = await _context.InstructionalMethods
                .Include(im => im.CourseSections)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (InstructionalMethod == null) return NotFound();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            InstructionalMethod = await _context.InstructionalMethods
                .Include(im => im.CourseSections)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (InstructionalMethod != null)
            {
                if (InUse) return RedirectToPage();
                
                _context.InstructionalMethods.Remove(InstructionalMethod);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}