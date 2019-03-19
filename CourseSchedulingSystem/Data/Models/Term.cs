using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Models
{
    public class Term
    {
        private string _name;
        private DateTime? _startDate, _endDate;

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

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
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
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
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

        public virtual ICollection<TermPart> TermParts { get; set; }
    }
}
