using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class AttributeType
    {
        public Guid Id { get; set; }

        [Required] public string Name { get; set; }
        public string NormalizedName { get; set; }

        public List<CourseAttributeType> CourseAttributeTypes { get; set; }
    }
}