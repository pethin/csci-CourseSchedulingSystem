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
        public Department Department { get; set; }

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Level
        {
            get => _level;
            set => _level = value.Trim().ToUpper();
        }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
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

        [Required]
        [Column(TypeName = "decimal(5, 3)")]
        public decimal CreditHours { get; set; }

        public List<CourseScheduleType> CourseScheduleTypes { get; set; }
        public List<CourseAttributeType> CourseAttributeTypes { get; set; }
    }
}
