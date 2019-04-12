using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using System.Collections.Async;

namespace CourseSchedulingSystem.Pages.Manage.RoomCapabilities
{
    public class CreateModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public CreateModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public RoomCapability RoomCapability { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var roomCapability = new RoomCapability();

            if (await TryUpdateModelAsync(roomCapability, "RoomCapability", cp => cp.Name))
            {
                await roomCapability.DbValidateAsync(_context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });

                if (!ModelState.IsValid) return Page();

                _context.RoomCapability.Add(RoomCapability);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}