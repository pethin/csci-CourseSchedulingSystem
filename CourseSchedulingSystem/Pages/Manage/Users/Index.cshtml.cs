using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IList<ApplicationUser> ApplicationUser { get; set; }

        public async Task OnGetAsync()
        {
            ApplicationUser = await _userManager.Users
                .OrderBy(u => u.NormalizedUserName)
                .ToListAsync();
        }
    }
}