﻿using System;
using System.Collections.Async;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        public IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any instructor has the same name
                if (await context.Instructors
                    .Where(i => i.Id != Id)
                    .Where(i => i.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"An instructor already exists with the name {FullName}."));
            });
        }

        private void UpdateNormalizedName()
        {
            NormalizedName = FullName.ToUpper();
        }
    }
}