using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseSchedulingSystem.Data.Models
{
    public class InstructionalMethod
    {
        private string _name;

        public InstructionalMethod()
        {
        }

        public InstructionalMethod(string name, bool requiresRoom = true)
        {
            Name = name;
            IsRoomRequired = requiresRoom;
        }

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

        [Required]
        [Display(Name = "Requires Room")]
        public bool IsRoomRequired { get; set; }
    }
}