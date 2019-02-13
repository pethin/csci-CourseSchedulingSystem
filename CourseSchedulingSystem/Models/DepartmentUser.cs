using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class DepartmentUser
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
