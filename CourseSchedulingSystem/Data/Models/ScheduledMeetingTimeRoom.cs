using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>
    /// Bridging entity for many-to-many relation between scheduled meeting times and rooms.
    /// </summary>
    public class ScheduledMeetingTimeRoom
    {
        /// <summary>Gets or sets the scheduled meeting time ID for this relation.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid ScheduledMeetingTimeId { get; set; }
        
        /// <summary>Navigation property for the scheduled meeting time this relation belongs to.</summary>
        public ScheduledMeetingTime ScheduledMeetingTime { get; set; }
        
        /// <summary>Gets or sets the room ID for this relation.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid RoomId { get; set; }
        
        /// <summary>Navigation property for the room this relation belongs to.</summary>
        public Room Room { get; set; }
    }
}
