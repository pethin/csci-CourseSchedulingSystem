using System;

namespace CourseSchedulingSystem.Data.Models
{
    public class DepartmentUser
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}