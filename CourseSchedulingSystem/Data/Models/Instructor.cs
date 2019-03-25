using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseSchedulingSystem.Data.Models
{
    public class Instructor
    {
        private string _firstName, _middle, _lastName;

        public Instructor()
        {
        }

        public Instructor(string firstName, string lastName, string middle = null)
        {
            FirstName = firstName;
            Middle = middle;
            LastName = lastName;
        }

        public Guid Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value.Trim();
                UpdateNormalizedName();
            }
        }

        public string Middle
        {
            get => _middle;
            set
            {
                _middle = value == null ? value : value.Trim();
                UpdateNormalizedName();
            }
        }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value.Trim();
                UpdateNormalizedName();
            }
        }

        public string NormalizedName { get; private set; }

        [NotMapped]
        public string FullName =>
            $@"{(FirstName == null ? "" : $"{FirstName} ")}{(Middle == null ? "" : $"{Middle} ")}{LastName}";

        private void UpdateNormalizedName()
        {
            NormalizedName = FullName.ToUpper();
        }
    }
}