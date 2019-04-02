using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CourseSchedulingSystem.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class CoursesPageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;

        public CoursesPageModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<SelectListItem> ScheduleTypeOptions => Context.ScheduleTypes.Select(st => new SelectListItem
        {
            Value = st.Id.ToString(),
            Text = st.Name
        });
        
        public IEnumerable<SelectListItem> CourseAttributeOptions => Context.CourseAttributes.Select(st => new SelectListItem
        {
            Value = st.Id.ToString(),
            Text = st.Name
        });

        protected void LoadDropdownData()
        {
            ViewData["DepartmentId"] = Context.Departments
                .Select(d => new SelectListItem {Value = d.Id.ToString(), Text = $"{d.Code} - {d.Name}"});

            ViewData["SubjectId"] = Context.Subjects
                .Select(d => new SelectListItem {Value = d.Id.ToString(), Text = $"{d.Code} - {d.Name}"});
        }
        
        public enum CourseLevelEnum
        {
            [Display(Name = "Undergraduate")]
            Undergraduate,
            [Display(Name = "Graduate")]
            Graduate
        }

        public class CourseInputModel : IValidatableObject
        {
            public Guid Id { get; set; }

            [Required]
            public Guid DepartmentId { get; set; }

            [Required]
            public Guid SubjectId { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
            public string Number { get; set; }

            [Required]
            public string Title { get; set; }

            [Required]
            [Range(0.000, double.MaxValue, ErrorMessage = "Credit hours must be between greater than or equal to zero.")]
            [Display(Name = "Credit Hours")]
            [DisplayFormat(DataFormatString = "{0:F3}", ApplyFormatInEditMode = true)]
            public decimal CreditHours { get; set; }
            
            [Display(Name = "Course Levels")]
            public IEnumerable<CourseLevelEnum> CourseLevels { get; set; } = new List<CourseLevelEnum>();
        
            [Display(Name = "Schedule Types")]
            public IEnumerable<Guid> ScheduleTypeIds { get; set; } = new List<Guid>();
        
            [Display(Name = "Course Attributes")]
            public IEnumerable<Guid> CourseAttributeIds { get; set; } = new List<Guid>();

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (!CourseLevels.Any())
                {
                    yield return new ValidationResult("You must select at least one level.", new [] {"CourseLevels"});
                }
            }
        }
    }
}