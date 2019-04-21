using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents an instructor.</summary>
    /// <remarks>
    /// <para>An instructor must have a unique ID and unique FullName.</para>
    /// <para>An instructor has many scheduled meeting times.</para>
    /// </remarks>
    public class Instructor
    {
        private string _firstName, _middle, _lastName;

        /// <summary>Creates an instructional method with default fields.</summary>
        public Instructor()
        {
        }

        /// <summary>Creates an instructional method with specified fields.</summary>
        public Instructor(string firstName, string lastName, string middle = null)
        {
            FirstName = firstName;
            Middle = middle;
            LastName = lastName;
        }

        /// <summary>Gets or sets the primary key for this instructor.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the first name for this instructor.</summary>
        [Required]
        [Display(Name = "First Name")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value?.Trim();
                UpdateNormalizedName();
            }
        }

        /// <summary>Gets or sets the middle name for this instructor.</summary>
        public string Middle
        {
            get => _middle;
            set
            {
                _middle = value?.Trim();
                UpdateNormalizedName();
            }
        }

        /// <summary>Gets or sets the last name for this instructor.</summary>
        [Required]
        [Display(Name = "Last Name")]
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value?.Trim();
                UpdateNormalizedName();
            }
        }

        /// <summary>Gets the normalized name for this building.</summary>
        /// <remarks>
        /// <para>This field is automated.</para>
        /// <para>This field must be unique.</para>
        /// <para>This field is indexed.</para>
        /// </remarks>
        public string NormalizedName { get; private set; }

        /// <summary>Gets the full name of this instructor.</summary>
        [NotMapped]
        [Display(Name = "Name")]
        public string FullName =>
            string.Join(" ",
                new List<String> {FirstName, Middle, LastName}
                    .Where(part => !string.IsNullOrWhiteSpace(part)));
        
        /// <summary>Gets or sets the active status for this instructor.</summary>
        [Required]
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
        
        /// <summary>Navigation property for the scheduled meeting time associations for this instructor.</summary>
        public List<ScheduledMeetingTimeInstructor> ScheduledMeetingTimeInstructors { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
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

        /// <summary>Updates the NormalizedName property.</summary>
        private void UpdateNormalizedName()
        {
            NormalizedName = FullName.ToUpperInvariant();
        }
    }
}