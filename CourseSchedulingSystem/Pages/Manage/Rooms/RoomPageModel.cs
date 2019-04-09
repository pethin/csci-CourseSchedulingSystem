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
    public class RoomPageModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext Context;

        public RoomPageModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            Context = context;
        }
        protected void LoadDropdownData()
        {
            ViewData["DepartmentId"] = Context.Departments
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name});
        }

    }
}