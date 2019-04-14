using System.Collections.Generic;
using System.Linq;
using CourseSchedulingSystem.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections.ScheduledMeetingTimes
{
    public class ScheduledMeetingTimesPageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;

        public ScheduledMeetingTimesPageModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<SelectListItem> MeetingTypeIds => Context.MeetingTypes
            .Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Code + " - " + mt.Name
            });
    }
}