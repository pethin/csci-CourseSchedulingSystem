using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Departments
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        public Department Department { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Department = await _context.Departments
                .Include(d => d.DepartmentUsers)
                .ThenInclude(du => du.User)
                .Include(d => d.Courses)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Department == null) return NotFound();
            return Page();
        }
    }
}