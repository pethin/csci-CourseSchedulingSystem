using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a department, e.g., CSQM.</summary>
    /// <remarks>
    /// <para>A department must have a unique ID, unique Code, and unique Name</para>
    /// <para>A course section has many users and courses.</para>
    /// </remarks>
    public class Department
    {
        private string _code;
        private string _name;

        /// <summary>Creates department with default fields.</summary>
        public Department()
        {
        }

        /// <summary>Creates a department with the specified fields.</summary>
        public Department(Guid id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
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

        /// <summary>Navigation property for the user associations this department has.</summary>
        public List<DepartmentUser> DepartmentUsers { get; set; }
        
        /// <summary>Navigation property for the courses this building has.</summary>
        public List<Course> Courses { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any department has the same code
                if (await context.Departments
                    .Where(d => d.Id != Id)
                    .Where(d => d.Code == Code)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A department already exists with the code {Code}."));

                // Check if any department has the same name
                if (await context.Departments
                    .Where(d => d.Id != Id)
                    .Where(d => d.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A department already exists with the name {Name}."));
            });
        }
    }
}