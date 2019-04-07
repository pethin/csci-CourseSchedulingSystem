using System;

namespace CourseSchedulingSystem.Data.Models
{
    public class CourseCourseAttribute
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid CourseAttributeId { get; set; }
        public CourseAttribute CourseAttribute { get; set; }
    }
}