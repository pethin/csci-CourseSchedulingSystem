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
        public AttributeType()
        {
        }

        public AttributeType(string name)
        {
            Name = name;
        }

        private string _name;

        public Guid Id { get; set; }

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value.Trim();
                NormalizedName = _name.ToUpper();
            }
        }

        public string NormalizedName { get; private set; }

        public List<CourseAttributeType> CourseAttributeTypes { get; set; }
    }
}