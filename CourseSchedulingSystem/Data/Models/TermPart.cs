using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class TermPart
    {
        private string _name;

        public TermPart()
        {
        }

        public TermPart(Term term, string name, DateTime startDate, DateTime endDate)
        {
            TermId = term.Id;
            Term = term;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public Guid Id { get; set; }

        public Guid TermId { get; set; }
        public virtual Term Term { get; set; }

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

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

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
                // Check if any term part has the same name
                if (await context.TermParts
                    .Where(tp => tp.Id != Id)
                    .Where(tp => tp.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A term part already exists with the name {Name}."));
            });
        }
    }
}