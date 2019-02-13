using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class ScheduleType
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public List<CourseScheduleType> CourseScheduleTypes { get; set; }
    }
}
