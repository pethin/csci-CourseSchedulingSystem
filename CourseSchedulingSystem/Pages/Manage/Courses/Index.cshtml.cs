using System.Collections.Generic;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class IndexModel : CoursesPageModel
    {
        public IndexModel(ApplicationDbContext context) : base(context)
        {
        }

        public IList<Course> Course { get; set; }

        public async Task OnGetAsync()
        {
            Course = await Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject)
                .ToListAsync();
        }
    }
}