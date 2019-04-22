using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections.ScheduledMeetingTimes
{
    public class ScheduledMeetingTimesPageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;

        public ScheduledMeetingTimesPageModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public CourseSection CourseSection { get; set; }

        public IEnumerable<SelectListItem> InstructorOptions => Context.Instructors
            .Where(i => i.IsActive || InstructorIds.Contains(i.Id))
            .Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.FullName
            });

        public IEnumerable<SelectListItem> RoomOptions => Context.Rooms
            .Include(r => r.Building)
            .Where(r => (r.Building.IsEnabled && r.IsEnabled) || RoomIds.Contains(r.Id))
            .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Building.Code + " " + r.Number
            });

        public IEnumerable<SelectListItem> MeetingTypeIds => Context.MeetingTypes
            .Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Code + " - " + mt.Name
            });

        [Display(Name = "Instructors")]
        [BindProperty]
        public IEnumerable<Guid> InstructorIds { get; set; } = new List<Guid>();

        [Display(Name = "Rooms")]
        [BindProperty]
        public IEnumerable<Guid> RoomIds { get; set; } = new List<Guid>();
    }
}