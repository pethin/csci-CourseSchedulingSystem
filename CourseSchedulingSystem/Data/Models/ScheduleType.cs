using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a schedule type, e.g., Lecture.</summary>
    /// <remarks>
    /// <para>A schedule type must have a unique ID, unique Code, and unique Name.</para>
    /// <para>A schedule type has many courses and course sections.</para>
    /// </remarks>
    public class ScheduleType
    {
        private string _code;
        private string _name;

        /// <summary>Creates a schedule type with default fields.</summary>
        public ScheduleType()
        {
        }

        /// <summary>Creates a schedule type with the specified fields.</summary>
        public ScheduleType(Guid id, string code, string name)
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

        /// <summary>Navigation property for the course associations for this schedule type.</summary>
        public List<CourseScheduleType> CourseScheduleTypes { get; set; }
        
        /// <summary>Navigation property for the course sections this schedule type has.</summary>
        public List<CourseSection> CourseSections { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any schedule type has the same code
                if (await context.ScheduleTypes
                    .Where(st => st.Id != Id)
                    .Where(st => st.Code == Code)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A schedule type already exists with the code {Code}."));
                
                // Check if any schedule type has the same name
                if (await context.ScheduleTypes
                    .Where(st => st.Id != Id)
                    .Where(st => st.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A schedule type already exists with the name {Name}."));
            });
        }
    }
}