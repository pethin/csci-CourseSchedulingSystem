using System;

namespace CourseSchedulingSystem.Data.Models
{
    public class CourseScheduleType
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid ScheduleTypeId { get; set; }
        public ScheduleType ScheduleType { get; set; }
    }
}