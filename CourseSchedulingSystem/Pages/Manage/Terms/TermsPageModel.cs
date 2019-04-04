using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class TermsPageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;

        public TermsPageModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public class TermInputModel
        {
            public Guid Id { get; set; }

            [Required] public string Name { get; set; }

            public List<TermPartInputModel> TermParts { get; set; } = new List<TermPartInputModel>();

            public void CheckForDuplicateTermParts(ModelStateDictionary modelState)
            {
                // Check for term parts with same names
                foreach (var duplicatedName in TermParts
                    .Where(tp => !string.IsNullOrWhiteSpace(tp.Name))
                    .GroupBy(tp => tp.Name)
                    .Where(models => models.Count() > 1))
                {
                    modelState.AddModelError(string.Empty,
                        $"There can not be more than one Part of Term with name '{duplicatedName.Key}'");
                }
            }
        }

        public class TermPartInputModel : IValidatableObject
        {
            private string _name;

            public Guid Id { get; set; }

            [Required]
            public string Name
            {
                get => _name;
                set
                {
                    _name = value?.Trim();
                    NormalizedName = _name?.ToUpper();
                }
            }

            public string NormalizedName { get; private set; }

            [Required]
            [Display(Name = "Start Date")]
            [DataType(DataType.Date)]
            public DateTime? StartDate { get; set; }

            [Required]
            [Display(Name = "End Date")]
            [DataType(DataType.Date)]
            public DateTime? EndDate { get; set; }
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (StartDate >= EndDate)
                {
                    yield return new ValidationResult("Start date must be before end date.", new[] {"StartDate"});
                    yield return new ValidationResult("End date must be after start date.", new[] {"EndDate"});
                }
            }
        }
    }
}