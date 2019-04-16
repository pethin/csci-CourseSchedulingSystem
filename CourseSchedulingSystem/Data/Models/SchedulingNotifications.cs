using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseSchedulingSystem.Data.Models
{
    public class SchedulingNotifications
    {
        [Required]
        public List<string> Warnings { get; set; } = new List<string>();
        
        [Required]
        public List<string> Errors { get; set; } = new List<string>();
    }
}