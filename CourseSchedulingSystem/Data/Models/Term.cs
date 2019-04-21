using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a term, e.g., Spring 2019.</summary>
    /// <remarks>
    /// <para>A term must have a unique ID and unique Name.</para>
    /// <para>A term has many term parts.</para>
    /// </remarks>
    public class Term
    {
        private string _name;

        /// <summary>Creates a term with no name.</summary>
        public Term()
        {
        }

        /// <summary>Creates a term with the specified name.</summary>
        /// <param name="name">The name</param>
        public Term(string name)
        {
            Name = name;
        }

        /// <summary>Gets or sets the primary key for this term.</summary>
        public Guid Id { get; set; }

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

        /// <summary>Gets the start date for this term.</summary>
        /// <remarks>TermParts must be loaded.</remarks>
        [NotMapped]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate => TermParts?.OrderBy(tp => tp.StartDate).Select(tp => tp.StartDate).FirstOrDefault();

        /// <summary>Gets the end date for this term.</summary>
        /// <remarks>TermParts must be loaded.</remarks>
        [NotMapped]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate => TermParts?.OrderByDescending(tp => tp.EndDate).Select(tp => tp.EndDate).FirstOrDefault();

        // TODO: Actually use the flag to prevent modifications
        /// <summary>Gets or sets the archived flag for this term.</summary>
        [Required]
        [Display(Name = "Archived")]
        public bool IsArchived { get; set; }

        /// <summary>Navigation property for the term parts this term has.</summary>
        public List<TermPart> TermParts { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any term has the same name
                if (await context.Terms
                    .Where(t => t.Id != Id)
                    .Where(t => t.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A term already exists with the name {Name}."));
            });
        }
    }
}
