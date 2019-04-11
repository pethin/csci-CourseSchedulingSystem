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
using System.ComponentModel.DataAnnotations;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
{
    public class RoomPageModel : PageModel
    {
        protected readonly ApplicationDbContext _context;

        public RoomPageModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<SelectListItem> RoomCapabilityOptions => _context.RoomCapability.Select(st => new SelectListItem
        {
            Value = st.Id.ToString(),
            Text = st.Name
        });

        public class RoomInputModel
        {
            public Guid Id { get; set; }

            [Display(Name = "Room Capabilities")]
            public IEnumerable<Guid> RoomCapabilityIds { get; set; } = new List<Guid>();
        }

    }
}