using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms
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
            return Page();
        }

        [BindProperty]
        public Term Term { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var newTerm = new Term();

            if (await TryUpdateModelAsync(
                newTerm,
                "Term",
                t => t.Name, t => t.StartDate, t => t.EndDate))
            {
                // Check if any term has the same name
                if (await _context.Terms.AnyAsync(t => t.Name == newTerm.Name))
                    ModelState.AddModelError(string.Empty,
                        $"A term already exists with the name {newTerm.Name}.");

                if (!ModelState.IsValid) return Page();

                _context.Terms.Add(newTerm);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}