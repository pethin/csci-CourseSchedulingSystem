using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class CourseAttributeType
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid AttributeTypeId { get; set; }
        public AttributeType AttributeType { get; set; }
    }
}
