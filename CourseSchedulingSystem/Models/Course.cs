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
        public Guid Id { get; set; }

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; }

        [Required]
        public string Level { get; set; }
        public string NormalizedLevel { get; set; }

        [Required]
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 3)")]
        public decimal CreditHours { get; set; }

        public List<CourseScheduleType> CourseScheduleTypes { get; set; }
        public List<CourseAttributeType> CourseAttributeTypes { get; set; }
    }
}
