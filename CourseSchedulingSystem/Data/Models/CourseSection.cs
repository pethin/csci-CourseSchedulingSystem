using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NPOI.Util;

namespace CourseSchedulingSystem.Data.Models
{
    public class CourseSection
    {
        public Guid Id { get; set; }
        
        [Required]
        [Display(Name = "Part of Term")]
        public Guid TermPartId { get; set; }
        public TermPart TermPart { get; set; }
        
        [Required]
        [Display(Name = "Course")]
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Section must be between positive.")]
        [DisplayFormat(DataFormatString = "{0:D3}", ApplyFormatInEditMode = true)]
        public int Section { get; set; }
        
        [Required]
        [Display(Name = "Schedule Type")]
        public Guid ScheduleTypeId { get; set; }
        public ScheduleType ScheduleType { get; set; }
        
        [Required]
        [Display(Name = "Instructional Method")]
        public Guid InstructionalMethodId { get; set; }
        public InstructionalMethod InstructionalMethod { get; set; }
        
        public string Notes { get; set; }
        
        // TODO: Add Errors property
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Capacity must be zero or positive.")]
        [Display(Name = "Max Capacity")]
        public int MaximumCapacity { get; set; }
        
        public List<ScheduledMeetingTime> ScheduledMeetingTimes { get; set; }
        
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if term part ID is valid
                if (!await context.TermParts.AnyAsync(m => m.Id == TermPartId))
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
                    .Where(cs => cs.Id != Id)
                    .Where(cs => cs.CourseId == CourseId)
                    .Where(cs => cs.Section == Section)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A course section already exists for course {course?.Identifier} with section number {Section}."));
            });
        }
    }
}
