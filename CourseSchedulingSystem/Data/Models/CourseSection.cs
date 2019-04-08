using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Models
{
    public class CourseSection
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid TermPartId { get; set; }
        public TermPart TermPart { get; set; }
        
        [Required]
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        
        [Required]
        public int Section { get; set; }
        
        [Required]
        public Guid ScheduleTypeId { get; set; }
        public ScheduleType ScheduleType { get; set; }
        
        [Required]
        public Guid InstructionalMethodId { get; set; }
        public InstructionalMethod InstructionalMethod { get; set; }
        
        [Required]
        public int MaximumCapacity { get; set; }
        
        public List<ScheduledMeetingTime> ScheduledMeetingTimes { get; set; }
    }
}
