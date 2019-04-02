using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
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
        ViewData["BuildingId"] = new SelectList(_context.Building.Where(bd => bd.IsEnabled==true), "Id", "Code");
            return Page();
        }

        [BindProperty]
        public Room Room { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ViewData["BuildingId"] = new SelectList(_context.Building.Where(bd => bd.IsEnabled == true), "Id", "Code");

            var room = new Room();

            if (await TryUpdateModelAsync(room, "Room", rm => rm.Number, rm => rm.BuildingId))
            {
                await room.DbValidateAsync(_context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });


                if (!ModelState.IsValid) return Page();

                _context.Room.Add(Room);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}