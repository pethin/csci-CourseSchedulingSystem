using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.Util;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>A course section is the entity which students register for.</summary>
    /// <remarks>
    /// <para>A course section must have a unique ID, and unique (TermPart, Course, Section).</para>
    /// <para>A course section belongs to a TermPart, Course, Section, Schedule Type, and Instructional Method.</para>
    /// <para>A course section has many scheduled meeting times.</para>
    /// </remarks>
    public class CourseSection
    {
        /// <summary>Gets or sets the primary key for this course section.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the term part ID for this course section.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        [Required]
        [Display(Name = "Part of Term")]
        public Guid TermPartId { get; set; }

        /// <summary>Navigation property for the part of term this course section belongs to.</summary>
        public TermPart TermPart { get; set; }

        /// <summary>Gets or sets the course ID for this course section.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        [Required]
        [Display(Name = "Course")]
        public Guid CourseId { get; set; }

        /// <summary>Navigation property for the course this course section belongs to.</summary>
        public Course Course { get; set; }

        /// <summary>Gets or sets the section number for this course section.</summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Section must be between positive.")]
        [DisplayFormat(DataFormatString = "{0:D3}", ApplyFormatInEditMode = true)]
        public int Section { get; set; }

        /// <summary>Gets or sets the schedule type ID for this course section.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        [Required]
        [Display(Name = "Schedule Type")]
        public Guid ScheduleTypeId { get; set; }

        /// <summary>Navigation property for the schedule type this course section belongs to.</summary>
        public ScheduleType ScheduleType { get; set; }

        /// <summary>Gets or sets the instructional method ID for this course section.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        [Required]
        [Display(Name = "Instructional Method")]
        public Guid InstructionalMethodId { get; set; }

        /// <summary>Navigation property for the instructional method this course section belongs to.</summary>
        public InstructionalMethod InstructionalMethod { get; set; }

        /// <summary>Gets or sets the footnotes for this course section.</summary>
        public string Footnotes { get; set; }

        /// <summary>Internal column for scheduling notifications property.</summary>
        internal string _SchedulingNotifications { get; set; }
        
        /// <summary>Gets or sets the scheduling notifications for this course section.</summary>
        [NotMapped]
        public SchedulingNotifications SchedulingNotifications
        {
            get => _SchedulingNotifications == null
                ? null
                : JsonConvert.DeserializeObject<SchedulingNotifications>(_SchedulingNotifications);
            set => _SchedulingNotifications = JsonConvert.SerializeObject(value);
        }

        /// <summary>Gets or sets the maximum capacity for this course section.</summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Capacity must be zero or positive.")]
        [Display(Name = "Capacity")]
        public int MaximumCapacity { get; set; }

        /// <summary>Navigation property for the scheduled meeting times this course section has.</summary>
        public List<ScheduledMeetingTime> ScheduledMeetingTimes { get; set; }

        /// <summary>Returns validation errors for database constraints.</summary>
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if term part ID is valid
                var termPart = await context.TermParts.FirstOrDefaultAsync(m => m.Id == TermPartId);
                if (termPart == null)
                    await yield.ReturnAsync(new ValidationResult("Invalid part of term selected."));

                var course = await context.Courses
                    .Include(c => c.Subject)
                    .Where(m => m.Id == CourseId)
                    .FirstOrDefaultAsync();

                // Check if course ID is valid
                if (course == null)
                    await yield.ReturnAsync(new ValidationResult("Invalid course selected."));

                // Check if schedule type ID is valid
                if (!await context.ScheduleTypes.AnyAsync(m => m.Id == ScheduleTypeId))
                    await yield.ReturnAsync(new ValidationResult("Invalid schedule type selected."));

                // Check if instructional method ID is valid
                if (!await context.InstructionalMethods.AnyAsync(m => m.Id == InstructionalMethodId))
                    await yield.ReturnAsync(new ValidationResult("Invalid instructional method selected."));

                // Check if any course section has the same course and section
                if (await context.CourseSections
                    .Include(cs => cs.TermPart)
                    .Where(cs => cs.Id != Id)
                    .Where(cs => cs.TermPart.TermId == termPart.TermId)
                    .Where(cs => cs.CourseId == CourseId)
                    .Where(cs => cs.Section == Section)
                    .AnyAsync())
                {
                    await yield.ReturnAsync(
                        new ValidationResult(
                            $"A course section already exists for course {course?.Identifier} with section number {Section}."));
                }
            });
        }
    }
}