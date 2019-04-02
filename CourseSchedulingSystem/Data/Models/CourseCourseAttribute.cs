using System;

namespace CourseSchedulingSystem.Data.Models
{
    public class CourseCourseAttribute
    {
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public Guid CourseAttributeId { get; set; }
        public virtual CourseAttribute CourseAttribute { get; set; }
    }
}