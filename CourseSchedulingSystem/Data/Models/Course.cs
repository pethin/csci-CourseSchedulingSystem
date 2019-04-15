using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>
    /// Represents a course. A course belongs to a department and subject. A Course is scheduled as a CourseSection.
    /// </summary>
    public class Course : IValidatableObject
    {
        private string _number;
        private string _title;

        /// <summary>Gets or sets the primary key for this course.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the department ID for this course.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        [Required]
        [Display(Name = "Department")]
        public Guid DepartmentId { get; set; }
        
        /// <summary>Navigation property for the department this course belongs to.</summary>
        public Department Department { get; set; }

        /// <summary>Gets or sets the subject ID for this course.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        [Required]
        [Display(Name = "Subject")]
        public Guid SubjectId { get; set; }
        
        /// <summary>Navigation property for the subject this course belongs to.</summary>
        public Subject Subject { get; set; }

        /// <summary>Gets or sets the course number.</summary>
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Number
        {
            get => _number;
            set => _number = value?.Trim().ToUpperInvariant();
        }

        /// <summary>Gets or sets the title/name for this course.</summary>
        [Required]
        public string Title
        {
            get => _title;
            set => _title = value?.Trim();
        }

        /// <summary>Gets the identifier for the course.</summary>
        /// <remarks>Preferably use `Course.Subject.Code + Course.Number` in LINQ.</remarks>
        [NotMapped] public string Identifier => Subject?.Code + Number;

        /// <summary>Gets or sets the credit hours for this course.</summary>
        [Required]
        [Column(TypeName = "decimal(5, 3)")]
        [Range(0.000, double.MaxValue, ErrorMessage = "Credit hours must be between greater than or equal to zero.")]
        [Display(Name = "Credit Hours")]
        [DisplayFormat(DataFormatString = "{0:F3}", ApplyFormatInEditMode = true)]
        public decimal CreditHours { get; set; }

        /// <summary>Gets or sets the undergraduate requirement for this course.</summary>
        [Required]
        [Display(Name = "Undergraduate")]
        public bool IsUndergraduate { get; set; }

        /// <summary>Gets or sets the graduate requirement for this course.</summary>
        [Required]
        [Display(Name = "Graduate")]
        public bool IsGraduate { get; set; }

        /// <summary>Gets or sets the enabled flag for this course.</summary>
        [Required]
        [Display(Name = "Enabled")]
        public bool IsEnabled { get; set; }

        /// <summary>Navigation property for the schedule type associations for this course.</summary>
        public List<CourseScheduleType> CourseScheduleTypes { get; set; }
        
        /// <summary>Navigation property for the course attribute associations for this course.</summary>
        public List<CourseCourseAttribute> CourseCourseAttributes { get; set; }

        /// <summary>Returns validation errors this course excluding database constraints.</summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsUndergraduate && !IsGraduate)
                yield return new ValidationResult(
                    "A level must be selected.",
                    new[] {"IsUndergraduate", "IsGraduate"});
        }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if department ID is valid
                if (!await context.Departments.AnyAsync(s => s.Id == DepartmentId))
                    await yield.ReturnAsync(new ValidationResult("Invalid department selected."));

                // Check if subject ID is valid
                var subject = await context.Subjects.FirstOrDefaultAsync(s => s.Id == SubjectId);
                if (subject == null)
                    await yield.ReturnAsync(new ValidationResult("Invalid subject selected."));
                else
                    Subject = subject;

                // Check if any course has the same subject and level
                if (await context.Courses
                    .Where(c => c.Id != Id)
                    .Where(c => c.SubjectId == SubjectId)
                    .Where(c => c.Number == Number)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A course already exists with the identifier {Identifier}."));
            });
        }
    }
}