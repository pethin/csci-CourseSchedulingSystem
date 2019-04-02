using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Departments
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Department> Departments { get; set; }

        public async Task OnGetAsync()
        {
            Departments = await _context.Departments
                .OrderBy(d => d.Code)
                .ToListAsync();
        }
    }
}