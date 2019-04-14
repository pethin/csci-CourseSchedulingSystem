using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CourseSchedulingSystem.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class CourseSectionsPageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;

        public CourseSectionsPageModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<SelectListItem> CourseIds => Context.Courses
            .Include(c => c.Subject)
            .Where(c => c.IsEnabled)
            .OrderBy(c => c.Subject.Code)
            .ThenBy(c => c.Number)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Subject.Code + c.Number + " - " + c.Title
            });

        public IEnumerable<SelectListItem> InstructionalMethodIds => Context.InstructionalMethods
            .OrderBy(im => im.Code)
            .Select(im => new SelectListItem
            {
                Value = im.Id.ToString(),
                Text = im.Code + " - " + im.Name
            });

        public IEnumerable<SelectListItem> ScheduleTypeIds => Context.ScheduleTypes
            .OrderBy(im => im.Code)
            .Select(st => new SelectListItem
            {
                Value = st.Id.ToString(),
                Text = st.Code + " - " + st.Name
            });
        
        // TODO: Add Code to TermPart
        public IEnumerable<SelectListItem> TermPartIds { get; set; }
    }
}