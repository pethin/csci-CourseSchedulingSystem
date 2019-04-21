using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents an instructional method, e.g., Classroom or Online.</summary>
    /// <remarks>
    /// <para>An instructional method must have a unique ID, unique Code, and unique Name.</para>
    /// <para>An instructional method has many course sections.</para>
    /// </remarks>
    public class InstructionalMethod
    {
        private string _code;
        private string _name;

        /// <summary>Creates an instructional method with default fields.</summary>
        public InstructionalMethod()
        {
        }

        /// <summary>Creates an instructional method with the specified fields.</summary>
        public InstructionalMethod(Guid id, string code, string name, bool requiresRoom = true)
        {
            Id = id;
            Code = code;
            Name = name;
            IsRoomRequired = requiresRoom;
        }

        /// <summary>Gets or sets the primary key for this department.</summary>
        public Guid Id { get; set; }
        
        /// <summary>Gets or sets the code for this department.</summary>
        /// <remarks>This field must be unique.</remarks>
        /// <remarks>This field is indexed.</remarks>
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Code
        {
            get => _code;
            set => _code = value?.Trim().ToUpperInvariant();
        }

        /// <summary>Gets or sets the name for this department.</summary>
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
        /// <remarks>This field is automated.</remarks>
        /// <remarks>This field must be unique.</remarks>
        /// <remarks>This field is indexed.</remarks>
        public string NormalizedName { get; private set; }

        /// <summary>Gets or sets the room requirement for this instructional method.</summary>
        [Required]
        [Display(Name = "Requires Room")]
        public bool IsRoomRequired { get; set; }
        
        /// <summary>Navigation property for the course sections this instructional method has.</summary>
        public List<CourseSection> CourseSections { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any instructional method has the same code
                if (await context.InstructionalMethods
                    .Where(im => im.Id != Id)
                    .Where(im => im.Code == Code)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"An instructional methods already exists with the code {Code}."));
                
                // Check if any instructional method has the same name
                if (await context.InstructionalMethods
                    .Where(im => im.Id != Id)
                    .Where(im => im.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"An instructional methods already exists with the name {Name}."));
            });
        }
    }
}