using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class ScheduledMeetingTime : IValidatableObject
    {
        public Guid Id { get; set; }

        [Required] public Guid CourseSectionId { get; set; }
        public CourseSection CourseSection { get; set; }

        [Required]
        [Display(Name = "Meeting Type")]
        public Guid MeetingTypeId { get; set; }
        public MeetingType MeetingType { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeSpan? StartTime { get; set; }

        [NotMapped]
        [Display(Name = "Start Time")]
        public string StartTimeText
        {
            get
            {
                if (StartTime == null)
                {
                    return "TBA";
                }

                var dateTime = DateTime.Today + StartTime.Value;
                return dateTime.ToString("hh:mm tt");
            }
        }
        

        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public TimeSpan? EndTime { get; set; }
        
        [NotMapped]
        [Display(Name = "End Time")]
        public string EndTimeText
        {
            get
            {
                if (EndTime == null)
                {
                    return "TBA";
                }

                var dateTime = DateTime.Today + EndTime.Value;
                return dateTime.ToString("hh:mm tt");
            }
        }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        [NotMapped]
        [Display(Name = "Days")]
        public string DaysOfWeek
        {
            get
            {
                string days = "" +
                       (Monday ? "M" : "") +
                       (Tuesday ? "T" : "") +
                       (Wednesday ? "W" : "") +
                       (Thursday ? "R" : "") +
                       (Friday ? "F" : "") +
                       (Saturday ? "S" : "") +
                       (Sunday ? "U" : "");

                if (string.IsNullOrEmpty(days))
                {
                    return "TBA";
                }

                return days;
            }
        }

        public List<ScheduledMeetingTimeRoom> ScheduledMeetingTimeRooms { get; set; }

        public List<ScheduledMeetingTimeInstructor> ScheduledMeetingTimeInstructors { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime == null && EndTime != null)
            {
                yield return new ValidationResult("Start time is required if end time is defined.", new[] {"StartTime"});
            }
            
            if (StartTime != null && EndTime == null)
            {
                yield return new ValidationResult("Start time is required if end time is defined.", new[] {"EndTime"});
            }

            if (StartTime != null && EndTime != null)
            {
                if (StartTime >= EndTime)
                {
                    yield return new ValidationResult("Start time must be before end time.", new[] {"StartTime"});
                    yield return new ValidationResult("End time must be after start time.", new[] {"EndTime"});
                }                
            }
        }
        
        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {   
                // If the scheduled meeting time's meeting type is Class, then check if any other scheduled meeting
                // time has the same meeting type
                if (MeetingTypeId == MeetingType.ClassMeetingType.Id)
                {
                    if (await context.ScheduledMeetingTimes
                        .Where(smt => smt.Id != Id)
                        .Where(smt => smt.MeetingTypeId == MeetingType.ClassMeetingType.Id)
                        .AnyAsync())
                        await yield.ReturnAsync(
                            new ValidationResult($"A meeting time already exists with the class meeting type."));
                }
            });
        }
    }
}