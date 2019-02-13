using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class Subject
    {
        public Guid Id { get; set; }

        [Required]
        public string Code { get; set; }
        public string NormalizedCode { get; set; }

        [Required]
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public List<Course> Courses { get; set; }
    }
}
