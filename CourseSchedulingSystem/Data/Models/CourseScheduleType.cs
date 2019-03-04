using System;

namespace CourseSchedulingSystem.Data.Models
{
    public class CourseScheduleType
    {
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public Guid ScheduleTypeId { get; set; }
        public virtual ScheduleType ScheduleType { get; set; }
    }
}
