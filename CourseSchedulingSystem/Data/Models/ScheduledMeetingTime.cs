using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Models
{
    public class ScheduledMeetingTime
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CourseSectionId { get; set; }
        public CourseSection CourseSection { get; set; }

        [Required]
        public Guid MeetingTypeId { get; set; }
        public MeetingType MeetingType { get; set; }

        public DateTime? StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public List<ScheduledMeetingTimeRoom> ScheduledMeetingTimeRooms { get; set; }
        
        public List<ScheduledMeetingTimeInstructor> ScheduledMeetingTimeInstructors { get; set; }
    }
}
