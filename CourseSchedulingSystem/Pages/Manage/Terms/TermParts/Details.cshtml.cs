using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms.TermParts
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        public TermPart TermPart { get; set; }
        
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            TermPart = await _context.TermParts
                .Include(t => t.Term)
                .Include(t => t.CourseSections)
                .ThenInclude(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (TermPart == null)
            {
                return NotFound();
            }

            if (returnUrl == null || !Url.IsLocalUrl(returnUrl))
            {
                ReturnUrl = Url.Page("../Details", new {id = TermPart.TermId});
            }
            else
            {
                ReturnUrl = returnUrl;
            }

            return Page();
        }
    }
}