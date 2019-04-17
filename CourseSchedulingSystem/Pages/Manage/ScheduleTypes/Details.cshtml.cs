using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.ScheduleTypes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        public ScheduleType ScheduleType { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ScheduleType = await _context.ScheduleTypes
                .Include(st => st.CourseScheduleTypes)
                .ThenInclude(cst => cst.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (ScheduleType == null) return NotFound();
            return Page();
        }
    }
}