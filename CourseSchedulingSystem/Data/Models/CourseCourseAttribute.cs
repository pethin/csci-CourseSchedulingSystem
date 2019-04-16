using System;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>Bridging entity for many-to-many relation between courses and course attributes.</summary>
    public class CourseCourseAttribute
    {
        /// <summary>Gets or sets the course ID for this relation.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid CourseId { get; set; }
        
        /// <summary>Navigation property for the course this relation belongs to.</summary>
        public Course Course { get; set; }

        /// <summary>Gets or sets the course attribute ID for this relation.</summary>
        /// <remarks>This field is a foreign key relation.</remarks>
        public Guid CourseAttributeId { get; set; }
        
        /// <summary>Navigation property for the course attribute this relation belongs to.</summary>
        public CourseAttribute CourseAttribute { get; set; }
    }
}