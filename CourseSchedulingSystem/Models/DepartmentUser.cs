using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Models
{
    public class DepartmentUser
    {
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; }
    }
}
