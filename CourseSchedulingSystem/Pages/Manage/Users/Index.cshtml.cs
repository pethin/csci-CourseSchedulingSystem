using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ApplicationUser> ApplicationUser { get; set; }

        public async Task OnGetAsync()
        {
            ApplicationUser = await _context.Users
                .Include(u => u.DepartmentUsers)
                .ThenInclude(du => du.Department)
                .ToListAsync();
        }
    }
}