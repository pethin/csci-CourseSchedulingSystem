using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class CourseAttributeType
    {
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public Guid AttributeTypeId { get; set; }
        public virtual AttributeType AttributeType { get; set; }
    }
}
