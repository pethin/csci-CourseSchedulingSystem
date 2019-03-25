using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
                yield return new ValidationResult("Start date must be before end date.", new[] { "StartDate" });
                yield return new ValidationResult("End date must be after start date.", new[] { "EndDate" });
            }
        }
    }
}
