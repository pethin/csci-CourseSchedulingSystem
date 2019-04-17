using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a building, e.g., Thurmond Hall.</summary>
    /// <remarks>Buildings must have a unique ID, unique Code, and unique Name.</remarks>
    /// <remarks>A building has many rooms.</remarks>
    public class Building
    {
        private string _name;
        private string _code;

        /// <summary>Creates a building with default fields.</summary>
        public Building()
        {
        }

        /// <summary>Creates a building with the specified fields.</summary>
        public Building(Guid id, string code, string name, bool enabled = true)
        {
            Id = id;
            Code = code;
            Name = name;
            IsEnabled = enabled;
        }

        /// <summary>Gets or sets the primary key for this building.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the code for this building.</summary>
        /// <remarks>This field must be unique.</remarks>
        /// <remarks>This field is indexed.</remarks>
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

        /// <summary>Gets or sets the enabled flag for this building.</summary>
        [Required]
        [Display(Name = "Enabled")]
        public bool IsEnabled { get; set; }

        /// <summary>Navigation property for the rooms this building has.</summary>
        public List<Room> Rooms { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any other building has the same code
                if (await context.Buildings
                    .Where(bd => bd.Id != Id)
                    .Where(bd => bd.Code == Code)
                    .AnyAsync())
                {
                    await yield.ReturnAsync(
                        new ValidationResult($"A building already exists with the code {Code}."));
                }

                // Check if any other building has the same name
                if (await context.Buildings
                    .Where(bd => bd.Id != Id)
                    .Where(bd => bd.NormalizedName == NormalizedName)
                    .AnyAsync())
                {
                    await yield.ReturnAsync(
                        new ValidationResult($"A building already exists with the name {Name}."));
                }
            });
        }
    }
}