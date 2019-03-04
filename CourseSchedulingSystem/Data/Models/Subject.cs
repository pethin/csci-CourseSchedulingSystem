using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseSchedulingSystem.Data.Models
{
    public class Subject
    {
        private string _code;

        private string _name;

        public Subject()
        {
        }

        public Subject(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public Guid Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Code
        {
            get => _code;
            set => _code = value.Trim().ToUpper();
        }

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

        public virtual ICollection<Course> Courses { get; set; }
    }
}