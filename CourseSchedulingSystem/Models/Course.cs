using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class Course
    {
        private string _level;
        private string _title;

        public Guid Id { get; set; }

        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public Guid SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Level
        {
            get => _level;
            set => _level = value.Trim().ToUpper();
        }

        [Required]
        public string Title
        {
            get => _title;
            set
            {
                _title = value.Trim();
                NormalizedTitle = _title.ToUpper();
            }
        }
        public string NormalizedTitle { get; set; }

        [NotMapped]
        public string Identifier => $@"{Subject?.Code ?? $"{SubjectId.ToString()} - "}{Level}";

        [Required]
        [Column(TypeName = "decimal(5, 3)")]
        [Range(0.000, double.MaxValue, ErrorMessage = "Credit hours must be between greater than or equal to zero.")]
        [Display(Name = "Credit Hours")]
        [DisplayFormat(DataFormatString = "{0:F3}", ApplyFormatInEditMode = true)]
        public decimal CreditHours { get; set; }

        public virtual ICollection<CourseScheduleType> CourseScheduleTypes { get; set; }
        public virtual ICollection<CourseAttributeType> CourseAttributeTypes { get; set; }
    }
}
