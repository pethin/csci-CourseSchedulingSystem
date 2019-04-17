using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>An attribute that can be assigned to courses, e.g., Capstone Course.</summary>
    /// <remarks>
    /// <para>An attribute has many courses.</para>
    /// <para>An attribute must have a unique ID and a unique Name.</para>
    /// <para>Attributes do not affect scheduling.</para>
    /// </remarks>
    public class CourseAttribute
    {
        private string _name;

        /// <summary>Creates a course attribute with default fields.</summary>
        public CourseAttribute()
        {
        }

        /// <summary>Creates a course attribute with an ID and name.</summary>
        public CourseAttribute(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>Gets or sets the primary key for this course attribute.</summary>
        public Guid Id { get; set; }

        // <summary>Gets or sets the name for this course attribute.</summary>
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
        /// <remarks>This field is automated.</remarks>
        /// <remarks>This field must be unique.</remarks>
        /// <remarks>This field is indexed.</remarks>
        public string NormalizedName { get; private set; }

        /// <summary>Navigation property for the course attribute associations for this course.</summary>
        public List<CourseCourseAttribute> CourseCourseAttributes { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any course attribute has the same name
                if (await context.CourseAttributes
                    .Where(at => at.Id != Id)
                    .Where(at => at.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A course attribute already exists with the name {Name}."));
            });
        }
    }
}