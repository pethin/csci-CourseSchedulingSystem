using System.Collections.Generic;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.AttributeTypes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<AttributeType> AttributeTypes { get; set; }

        public async Task OnGetAsync()
        {
            AttributeTypes = await _context.AttributeTypes.ToListAsync();
        }
    }
}