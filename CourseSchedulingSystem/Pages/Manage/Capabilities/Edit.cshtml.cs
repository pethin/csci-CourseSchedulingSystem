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
using System.Collections.Async;

namespace CourseSchedulingSystem.Pages.Manage.Capabilities
{
    public class EditModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Capability Capability { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Capability = await _context.Capability.FirstOrDefaultAsync(m => m.Id == id);

            if (Capability == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Capability).State = EntityState.Modified;

            var cap = await _context.Capability.FindAsync(id);

            if (await TryUpdateModelAsync(cap, "Capability", cp => cp.Name))
            {
                await cap.DbValidateAsync(_context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });


                if (!ModelState.IsValid) return Page();
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }

        private bool CapabilityExists(Guid id)
        {
            return _context.Capability.Any(e => e.Id == id);
        }
    }
}
