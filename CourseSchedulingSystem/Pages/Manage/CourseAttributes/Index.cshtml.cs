using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseAttributes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<CourseAttribute> AttributeTypes { get; set; }

        public async Task OnGetAsync()
        {
            AttributeTypes = await _context.CourseAttributes
                .OrderBy(at => at.NormalizedName)
                .ToListAsync();
        }
    }
}