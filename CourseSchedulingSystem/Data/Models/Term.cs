using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class Term : IValidatableObject
    {
        private string _name;

        public Term()
        {
        }

        public Term(string name, DateTime startDate, DateTime endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public Guid Id { get; set; }

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value.Trim();
                NormalizedName = _name.ToUpper();
            }
        }

        public string NormalizedName { get; private set; }

        // TODO: Remove start date and end date from term. Calculate from TermParts.
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public virtual ICollection<TermPart> TermParts { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate >= EndDate)
            {
                yield return new ValidationResult("Start date must be before end date.", new[] {"StartDate"});
                yield return new ValidationResult("End date must be after start date.", new[] {"EndDate"});
            }
        }

        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any term has the same name
                if (await context.Terms
                    .Where(t => t.Id != Id)
                    .Where(t => t.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A term already exists with the name {Name}."));
            });
        }
    }
}