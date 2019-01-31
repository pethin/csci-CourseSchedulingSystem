using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}
