using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseSchedulingSystem.Data.Models
{
    public class Course
    {
        private string _number;
        private string _title;

        public Guid Id { get; set; }

        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public Guid SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Number
        {
            get => _number;
            set => _number = value.Trim().ToUpper();
        }

        [Required]
        public string Title
        {
            get => _title;
            set => _title = value.Trim();
        }

        [NotMapped] public string Identifier => $@"{Subject?.Code ?? $"{SubjectId.ToString()} - "}{Number}";

        [Required]
        [Column(TypeName = "decimal(5, 3)")]
        [Range(0.000, double.MaxValue, ErrorMessage = "Credit hours must be between greater than or equal to zero.")]
        [Display(Name = "Credit Hours")]
        [DisplayFormat(DataFormatString = "{0:F3}", ApplyFormatInEditMode = true)]
        public decimal CreditHours { get; set; }

        [Required]
        [Display(Name = "Undergraduate")]
        public bool IsUndergraduate { get; set; }

        [Required]
        [Display(Name = "Graduate")]
        public bool IsGraduate { get; set; }

        public virtual ICollection<CourseScheduleType> CourseScheduleTypes { get; set; }
        public virtual ICollection<CourseAttributeType> CourseAttributeTypes { get; set; }
    }
}