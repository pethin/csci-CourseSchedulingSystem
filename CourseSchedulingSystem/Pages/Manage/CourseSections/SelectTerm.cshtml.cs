using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class SelectTermModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SelectTermModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [BindProperty]
        [Required]
        [Display(Name = "Term")]
        public Guid? TermId { get; set; }

        public IEnumerable<SelectListItem> TermsOptions => _context.Terms
            .OrderByDescending(term => term.StartDate)
            .ThenBy(term => term.Name)
            .Select(term => new SelectListItem
            {
                Value = term.Id.ToString(),
                Text = term.Name
            });

        public void OnGetAsync()
        {
        }

        public async Task<IActionResult> OnPostAsync(string handler)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var term = await _context.Terms
                .Where(t => t.Id == TermId)
                .FirstOrDefaultAsync();

            if (term == null)
            {
                ModelState.AddModelError(string.Empty, $"Could not find term with id: {TermId}");
                return Page();
            }

            switch (handler)
            {
                case "AddSection":
                    return RedirectToPage("Create", new {termId = TermId});
                case "ManageSections":
                    return RedirectToPage("Index", new {termId = TermId});
                default:
                    ModelState.AddModelError(string.Empty, $"Unknown page handler: {handler}.");
                    return Page();
            }
        }
    }
}