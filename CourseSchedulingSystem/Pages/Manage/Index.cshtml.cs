using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseSchedulingSystem.Pages.Manage
{
    public class IndexModel : PageModel
    {
        public List<Section> Sections { get; set; }
        
        public IndexModel()
        {
        }

        public void OnGet()
        {
            Sections = new List<Section>
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
                            AddLink = Url.Page("InstructionalMethods/Create"),
                            ManageLink = Url.Page("InstructionalMethods/Index")
                        },
                        new Row
                        {
                            Title = "Terms",
                            Description =
                                $"Terms are largest units of scheduling. E.g., Spring {DateTime.Now.Year}, Fall {DateTime.Now.Year}. Terms contain Parts of Terms.",
                            AddLink = Url.Page("Terms/Create"),
                            ManageLink = Url.Page("Terms/Index")
                        },
                        new Row
                        {
                            Title = "Schedule Courses",
                            Description = "Schedule course sections and add scheduled meeting times.",
                            AddLink = Url.Page("CourseSections/SelectTerm", "AddSection"),
                            ManageLink = Url.Page("CourseSections/SelectTerm", "ManageSections")
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
                            AddLink = Url.Page("Subjects/Create"),
                            ManageLink = Url.Page("Subjects/Index")
                        },
                        new Row
                        {
                            Title = "Course Attributes",
                            Description =
                                "Additional attributes that can be added to courses or course sections. E.g., Intensive Writing, Oral Communication.",
                            AddLink = Url.Page("CourseAttributes/Create"),
                            ManageLink = Url.Page("CourseAttributes/Index")
                        },
                        new Row
                        {
                            Title = "Schedule Types",
                            Description =
                                "Additional attributes that can be added to courses. E.g., Independent Study/Research, Lecture.",
                            AddLink = Url.Page("ScheduleTypes/Create"),
                            ManageLink = Url.Page("ScheduleTypes/Index")
                        },
                        new Row
                        {
                            Title = "Courses",
                            Description =
                                "Courses are scheduled as course sections. A course includes a subject, level, and credit hours.",
                            AddLink = Url.Page("Courses/Create"),
                            ManageLink = Url.Page("Courses/Index")
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
                            AddLink = Url.Page("Departments/Create"),
                            ManageLink = Url.Page("Departments/Index")
                        },
                        new Row
                        {
                            Title = "Instructors",
                            Description = "Instructors are assigned to scheduled meeting times.",
                            AddLink = Url.Page("Instructors/Create"),
                            ManageLink = Url.Page("Instructors/Index")
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
                            Title = "Buildings and Rooms",
                            Description = "Buildings contain rooms which are assigned to scheduled meeting times.",
                            AddLink = Url.Page("Buildings/Create"),
                            ManageLink = Url.Page("Buildings/Index")
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
                            AddLink = Url.Page("Users/Create"),
                            ManageLink = Url.Page("Users/Index")
                        }
                    }
                }
            };
        }

        public class Section
        {
            public string Title { get; set; }
            public IList<Row> Rows { get; set; } = new List<Row>();
        }

        public class Row
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string AddLink { get; set; }
            public string ManageLink { get; set; }
        }
    }
}