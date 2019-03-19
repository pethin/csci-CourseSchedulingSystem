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
        private DateTime? _startDate, _endDate;

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

        [Required]
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (EndDate != null && value >= EndDate)
                {
                    throw new ArgumentException("StartDate must be less than EndDate");
                }

                _startDate = value;
            }
        }

        [Required]
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (StartDate != null && value <= StartDate)
                {
                    throw new ArgumentException("EndDate must be greater than StartDate");
                }

                _endDate = value;
            }
        }

        public string NormalizedName { get; private set; }
    }
}
