using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class IndexModel : CoursesPageModel
    {
        public IndexModel(ApplicationDbContext context) : base(context)
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public class DataGridResult<T>
        {
            public List<T> Records { get; set; }
            public int Total { get; set; }
        }

        public class RecordLinks
        {
            public string Edit { get; set; }
            public string Details { get; set; }
            public string Delete { get; set; }
        }

        public class CourseRecord
        {
            public Guid Id { get; set; }
            public string Department { get; set; }
            public string Identifier { get; set; }
            public string Title { get; set; }
            public decimal CreditHours { get; set; }
            public RecordLinks Links { get; set; }
        }

        public async Task<IActionResult> OnGetDataAsync(
            int? pageNumber, int? limit,
            string sortBy,
            string direction,
            string department,
            string identifier,
            string title,
            decimal? creditHours)
        {
            var query = Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject)
                .Select(c => new CourseRecord
                {
                    Id = c.Id,
                    Department = c.Department.Code,
                    Identifier = c.Subject.Code + c.Number,
                    Title = c.Title,
                    CreditHours = c.CreditHours,
                    Links = new RecordLinks
                    {
                        Edit = Url.Page("Edit", new { id = c.Id }),
                        Details = Url.Page("Details", new { id = c.Id }),
                        Delete = Url.Page("Delete", new { id = c.Id })
                    }
                });

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(q => q.Department.Contains(department));
            }

            if (!string.IsNullOrWhiteSpace(identifier))
            {
                query = query.Where(q => q.Identifier.Contains(identifier));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(q => q.Title.Contains(title));
            }

            if (creditHours.HasValue)
            {
                query = query.Where(q => q.CreditHours == creditHours);
            }

            var total = query.Count();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim().ToLower())
                    {
                        case "department":
                            query = query.OrderBy(q => q.Department).ThenBy(q => q.Identifier);
                            break;
                        case "identifier":
                            query = query.OrderBy(q => q.Identifier);
                            break;
                        case "title":
                            query = query.OrderBy(q => q.Title);
                            break;
                        case "creditHours":
                            query = query.OrderBy(q => q.CreditHours);
                            break;
                    }
                }
                else
                {
                    switch (sortBy.Trim().ToLower())
                    {
                        case "department":
                            query = query.OrderByDescending(q => q.Department).ThenBy(q => q.Identifier);
                            break;
                        case "identifier":
                            query = query.OrderByDescending(q => q.Identifier);
                            break;
                        case "title":
                            query = query.OrderByDescending(q => q.Title);
                            break;
                        case "creditHours":
                            query = query.OrderByDescending(q => q.CreditHours);
                            break;
                    }
                }
            }
            else
            {
                query = query.OrderBy(q => q.Department).ThenBy(q => q.Identifier);
            }

            List<CourseRecord> records;
            if (pageNumber.HasValue && limit.HasValue)
            {
                int start = (pageNumber.Value - 1) * limit.Value;
                records = await query.Skip(start).Take(limit.Value).ToListAsync();
            }
            else
            {
                records = await query.ToListAsync();
            }

            return new JsonResult(new DataGridResult<CourseRecord>
            {
                Records = records,
                Total = total
            });
        }
    }
}