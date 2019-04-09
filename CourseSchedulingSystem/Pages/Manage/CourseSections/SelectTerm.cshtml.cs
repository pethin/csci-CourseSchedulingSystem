using System;
using System.Collections.Generic;
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
        public Guid TermId { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            return Page();
        }
    }
}