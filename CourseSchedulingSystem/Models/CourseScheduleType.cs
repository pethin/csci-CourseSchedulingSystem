using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class CourseScheduleType
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid ScheduleTypeId { get; set; }
        public ScheduleType ScheduleType { get; set; }
    }
}
