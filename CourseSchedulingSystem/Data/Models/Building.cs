using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Models
{
    public class Building
    {
        private string _name;
        private string _code;

        public Building()
        {
        }

        public Building(string name, string code, bool isEnabled)
        {
            Name = name;
            Code = code;
            IsEnabled = isEnabled;
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

        [Required]
        [Display (Name = "Building Enabled")]
        public bool IsEnabled { get; set; }

        public string NormalizedName { get; private set; }

    }
}