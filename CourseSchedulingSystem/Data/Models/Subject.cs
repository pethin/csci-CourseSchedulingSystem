using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a subject, e.g., Computer Science.</summary>
    /// <remarks>
    /// <para>A subject must have a unique ID, unique Code, and unique Name.</para>
    /// <para>A subject has many courses.</para>
    /// </remarks>
    public class Subject
    {
        private string _code;
        private string _name;

        /// <summary>Creates a subject with default fields.</summary>
        public Subject()
        {
        }

        /// <summary>Creates a subject with the specified fields.</summary>
        public Subject(Guid id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }

        /// <summary>Gets or sets the primary key for this schedule type.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the code for this building.</summary>
        /// <remarks>
        /// <para>This field must be unique.</para>
        /// <para>This field is indexed.</para>
        /// </remarks>
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Code
        {
            get => _code;
            set => _code = value?.Trim().ToUpperInvariant();
        }

        /// <summary>Gets or sets the name for this building.</summary>
        /// <remarks>The normalized version of this field must be unique.</remarks>
        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value?.Trim();
                NormalizedName = _name?.ToUpperInvariant();
            }
        }

        /// <summary>Gets the normalized name for this building.</summary>
        /// <remarks>
        /// <para>This field is automated.</para>
        /// <para>This field must be unique.</para>
        /// <para>This field is indexed.</para>
        /// </remarks>
        public string NormalizedName { get; private set; }

        /// <summary>Navigation property for the course sections this schedule type has.</summary>
        public List<Course> Courses { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any subject has the same code
                if (await context.Subjects
                    .Where(s => s.Id != Id)
                    .Where(s => s.Code == Code)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A subject already exists with the code {Code}."));

                // Check if any subject has the same name
                if (await context.Subjects
                    .Where(s => s.Id != Id)
                    .Where(s => s.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A subject already exists with the name {Name}."));
            });
        }
    }
}