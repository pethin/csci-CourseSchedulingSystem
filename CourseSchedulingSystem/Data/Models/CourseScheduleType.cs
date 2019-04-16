using System;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Bridging entity for many-to-many relation between courses and schedule types.</summary>
    public class CourseScheduleType
    {
        /// <summary>Gets or sets the course ID for this relation.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid CourseId { get; set; }
        
        /// <summary>Navigation property for the course this relation belongs to.</summary>
        public Course Course { get; set; }

        /// <summary>Gets or sets the schedule type ID for this relation.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid ScheduleTypeId { get; set; }
        
        /// <summary>Navigation property for the schedule type this relation belongs to.</summary>
        public ScheduleType ScheduleType { get; set; }
    }
}