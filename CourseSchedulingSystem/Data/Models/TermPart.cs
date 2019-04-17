using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a term part, e.g., Full Semester.</summary>
    /// <remarks>
    /// <para>A term must have a unique ID and unique (Term, Name).</para>
    /// <para>A term part has many course sections.</para>
    /// </remarks>
    public class TermPart : IValidatableObject
    {
        private string _name;

        /// <summary>Creates a term part with no name, start date, or end date.</summary>
        public TermPart()
        {
        }

        /// <summary>Creates a term part with the specified parameters.</summary>
        public TermPart(Term term, string name, DateTime startDate, DateTime endDate)
        {
            TermId = term.Id;
            Term = term;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>Gets or sets the primary key for this term part.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the term ID for this term part.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid TermId { get; set; }
        
        /// <summary>Navigation property for the term this term part belongs to.</summary>
        public Term Term { get; set; }

        /// <summary>Gets or sets the name for this term.</summary>
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

        /// <summary>Gets the normalized name for this term.</summary>
        /// <remarks>
        /// <para>This field is automated.</para>
        /// <para>This field must be unique.</para>
        /// <para>This field is indexed.</para>
        /// </remarks>
        public string NormalizedName { get; private set; }

        /// <summary>Gets or sets the start date for this term part.</summary>
        /// <remarks>This field is indexed.</remarks>
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        /// <summary>Gets or sets the end date for this term part.</summary>
        /// <remarks>This field is indexed.</remarks>
        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        
        /// <summary>Navigation property for the course sections this term has.</summary>
        public List<CourseSection> CourseSections { get; set; }

        /// <summary>Returns validation errors this term part excluding database constraints.</summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate >= EndDate)
            {
                yield return new ValidationResult("Start date must be before end date.", new[] {"StartDate"});
                yield return new ValidationResult("End date must be after start date.", new[] {"EndDate"});
            }
        }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any term part has the same name
                if (await context.TermParts
                    .Where(tp => tp.Id != Id)
                    .Where(tp => tp.TermId == TermId)
                    .Where(tp => tp.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A term part already exists with the name {Name}."));
            });
        }
    }
}