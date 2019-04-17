using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SelectList BuildingOptions =>
            new SelectList(_context.Buildings.Where(bd => bd.IsEnabled), "Id", "Code");

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public Room Room { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var room = new Room();

            if (await TryUpdateModelAsync(room, "Room", rm => rm.Number, rm => rm.BuildingId))
            {
                await room.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                _context.Rooms.Add(Room);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}