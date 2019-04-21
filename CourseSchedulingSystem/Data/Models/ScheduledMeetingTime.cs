using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Represents a scheduled meeting time for a course section.</summary>
    /// <remarks>
    /// <para>A scheduled meeting time must have a unique ID.</para>
    /// <para>A scheduled meeting time belongs to a course section and meeting type.</para>
    /// <para>A scheduled meeting time has many instructors and rooms.</para>
    /// </remarks>
    public class ScheduledMeetingTime : IValidatableObject
    {
        /// <summary>Gets or sets the primary key for this course section.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the course section ID for this scheduled meeting time.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        [Required]
        public Guid CourseSectionId { get; set; }

        /// <summary>Navigation property for the course section this scheduled meeting time belongs to.</summary>
        public CourseSection CourseSection { get; set; }

        /// <summary>Gets or sets the course section ID for this scheduled meeting time.</summary>
        [Required]
        [Display(Name = "Meeting Type")]
        public Guid MeetingTypeId { get; set; }

        /// <summary>Navigation property for the meeting type this scheduled meeting time belongs to.</summary>
        public MeetingType MeetingType { get; set; }

        /// <summary>Gets or sets the start time for this scheduled meeting time.</summary>
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeSpan? StartTime { get; set; }

        /// <summary>Gets the formatted 12 hour start time for this scheduled meeting time.</summary>
        /// <returns>"TBA" if no start time.</returns>
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


        /// <summary>Gets or sets the end time for this scheduled meeting time.</summary>
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public TimeSpan? EndTime { get; set; }

        /// <summary>Gets the formatted 12 hour end time for this scheduled meeting time.</summary>
        /// <returns>"TBA" if no start time.</returns>
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

        /// <summary>Gets or sets the Monday status for this scheduled meeting time.</summary>
        public bool Monday { get; set; }
        
        /// <summary>Gets or sets the Tuesday status for this scheduled meeting time.</summary>
        public bool Tuesday { get; set; }
        
        /// <summary>Gets or sets the Wednesday status for this scheduled meeting time.</summary>
        public bool Wednesday { get; set; }
        
        /// <summary>Gets or sets the Thursday status for this scheduled meeting time.</summary>
        public bool Thursday { get; set; }
        
        /// <summary>Gets or sets the Friday status for this scheduled meeting time.</summary>
        public bool Friday { get; set; }
        
        /// <summary>Gets or sets the Saturday status for this scheduled meeting time.</summary>
        public bool Saturday { get; set; }
        
        /// <summary>Gets or sets the Sunday status for this scheduled meeting time.</summary>
        public bool Sunday { get; set; }

        /// <summary>Gets the formatted days of week characters for this scheduled meeting time.</summary>
        /// <returns>"TBA" if not scheduled on any days.</returns>
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

        /// <summary>Navigation property for the rooms associations for this scheduled meeting time.</summary>
        public List<ScheduledMeetingTimeRoom> ScheduledMeetingTimeRooms { get; set; } =
            new List<ScheduledMeetingTimeRoom>();

        /// <summary>Gets a comma-delimited string of rooms for this scheduled meeting time.</summary>
        /// <remarks>The ScheduledMeetingTimeRooms.Room.Building properties must be initialized.</remarks>
        /// <returns>"TBA" if no rooms.</returns>
        [NotMapped]
        public string RoomsText
        {
            get
            {
                if (ScheduledMeetingTimeRooms.Count == 0)
                {
                    return "TBA";
                }

                return string.Join(", ", ScheduledMeetingTimeRooms.Select(smtr => smtr.Room.Identifier));
            }
        }

        /// <summary>Navigation property for the instructor associations for this scheduled meeting time.</summary>
        public List<ScheduledMeetingTimeInstructor> ScheduledMeetingTimeInstructors { get; set; } =
            new List<ScheduledMeetingTimeInstructor>();

        /// <summary>Gets a comma-delimited string of instructors for this scheduled meeting time.</summary>
        /// <remarks>The ScheduledMeetingTimeRooms.Instructor properties must be initialized.</remarks>
        /// <returns>"TBA" if no instructors.</returns>
        [NotMapped]
        public string InstructorsText
        {
            get
            {
                if (ScheduledMeetingTimeInstructors.Count == 0)
                {
                    return "TBA";
                }

                return string.Join(", ", ScheduledMeetingTimeInstructors.Select(smti => smti.Instructor.FullName));
            }
        }

        /// <summary>Internal serialized JSON of notifications object for this scheduled meeting time.</summary>
        internal string _SchedulingNotifications { get; set; }

        /// <summary>Gets or sets the notifications for this scheduled meeting time.</summary>
        /// <remarks>After modifying the notifications this setter must be used to actually store the changes.</remarks>
        [NotMapped]
        public SchedulingNotifications SchedulingNotifications
        {
            get => _SchedulingNotifications == null
                ? null
                : JsonConvert.DeserializeObject<SchedulingNotifications>(_SchedulingNotifications);
            set => _SchedulingNotifications = JsonConvert.SerializeObject(value);
        }

        /// <summary>Returns validation errors this scheduled meeting time excluding database constraints.</summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime == null && EndTime != null)
            {
                yield return new ValidationResult("Start time is required if end time is defined.",
                    new[] {"StartTime"});
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

        /// <summary>Returns validation errors for database constraints.</summary>
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