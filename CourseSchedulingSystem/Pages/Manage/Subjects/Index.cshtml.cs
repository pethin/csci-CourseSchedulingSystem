using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Subjects
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Subject> Subjects { get; set; }

        public async Task OnGetAsync()
        {
            Subjects = await _context.Subjects
                .OrderBy(s => s.Code)
                .ToListAsync();
        }
    }
}