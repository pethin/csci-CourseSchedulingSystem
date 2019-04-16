using System;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Bridging entity for many-to-many relation between departments and users.</summary>
    public class DepartmentUser
    {
        /// <summary>Gets or sets the user ID for this relation.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid UserId { get; set; }
        
        /// <summary>Navigation property for the user this relation belongs to.</summary>
        public ApplicationUser User { get; set; }

        /// <summary>Gets or sets the department ID for this relation.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid DepartmentId { get; set; }
        
        /// <summary>Navigation property for the department this relation belongs to.</summary>
        public Department Department { get; set; }
    }
}