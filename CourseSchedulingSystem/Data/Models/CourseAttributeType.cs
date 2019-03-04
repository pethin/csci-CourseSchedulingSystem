using System;

namespace CourseSchedulingSystem.Data.Models
{
    public class CourseAttributeType
    {
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public Guid AttributeTypeId { get; set; }
        public virtual AttributeType AttributeType { get; set; }
    }
}
