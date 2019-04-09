using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Models
{
    public class ScheduledMeetingTimeRoom
    {
        public Guid ScheduledMeetingTimeId { get; set; }
        public ScheduledMeetingTime ScheduledMeetingTime { get; set; }
        
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
    }
}
