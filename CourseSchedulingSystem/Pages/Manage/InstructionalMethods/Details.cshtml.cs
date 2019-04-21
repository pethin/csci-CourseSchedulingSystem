using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.InstructionalMethods
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }

        public InstructionalMethod InstructionalMethod { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            InstructionalMethod = await _context.InstructionalMethods.FirstOrDefaultAsync(m => m.Id == Id);

            if (InstructionalMethod == null) return NotFound();
            return Page();
        }
    }
}