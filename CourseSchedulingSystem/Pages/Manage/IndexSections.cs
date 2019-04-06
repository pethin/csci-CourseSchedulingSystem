using System;
using System.Collections.Generic;

namespace CourseSchedulingSystem.Pages.Manage
{
    public static class IndexSections
    {
        public static IList<Section> Sections = new List<Section>
        {
            new Section
            {
                Title = "Scheduling",
                Rows = new List<Row>
                {
                    new Row
                    {
                        Title = "Instructional Methods",
                        Description = "Instructional methods for course sections. E.g., Classroom, Hybrid, Online.",
                        Link = "./InstructionalMethods"
                    },
                    new Row
                    {
                        Title = "Terms",
                        Description =
                            $"Course sections are grouped by terms. E.g., Spring {DateTime.Now.Year}, Fall {DateTime.Now.Year}.",
                        Link = "./Terms"
                    }
                }
            },
            new Section
            {
                Title = "Course Management",
                Rows = new List<Row>
                {
                    new Row
                    {
                        Title = "Subjects",
                        Description = "Subjects are assigned to courses.",
                        Link = "./Subjects"
                    },
                    new Row
                    {
                        Title = "Course Attributes",
                        Description =
                            "Additional attributes that can be added to courses or course sections. E.g., Intensive Writing, Oral Communication.",
                        Link = "./CourseAttributes"
                    },
                    new Row
                    {
                        Title = "Schedule Types",
                        Description =
                            "Additional attributes that can be added to courses. E.g., Independent Study/Research, Lecture.",
                        Link = "./ScheduleTypes"
                    },
                    new Row
                    {
                        Title = "Courses",
                        Description =
                            "Courses are scheduled as course sections. A course includes a subject, level, and credit hours.",
                        Link = "./Courses"
                    }
                }
            },
            new Section
            {
                Title = "Department Management",
                Rows = new List<Row>
                {
                    new Row
                    {
                        Title = "Departments",
                        Description = "Courses and instructors are grouped by departments.",
                        Link = "./Departments"
                    },
                    new Row
                    {
                        Title = "Instructors",
                        Description = "Instructors are assigned to course sections.",
                        Link = "./Instructors"
                    }
                }
            },
            new Section
            {
                Title = "Infrastructure",
                Rows = new List<Row>
                {
                    new Row
                    {
                        Title = "Buildings",
                        Description = "Buildings contain rooms.",
                        Link = "./Buildings"
                    },
                    new Row
                    {
                        Title = "Rooms",
                        Description = "Rooms are assigned to applicable course sections.",
                        Link = "./Rooms"
                    },
                    new Row
                    {
                        Title = "Room Capabilities",
                        Description = "Capabilities and features are assigned to rooms.",
                        Link = "./Capabilities"
                    }
                }
            },
            new Section
            {
                Title = "Authentication and Authorization",
                Rows = new List<Row>
                {
                    new Row
                    {
                        Title = "Users",
                        Description = "Not related to instructors. Users for this course scheduling system.",
                        Link = "./Users"
                    },
                    new Row
                    {
                        Title = "Groups",
                        Description = "User groups are used to manage permissions."
                    }
                }
            }
        };

        public class Section
        {
            public string Title { get; set; }
            public IList<Row> Rows { get; set; } = new List<Row>();
        }

        public class Row
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Link { get; set; }
        }
    }
}