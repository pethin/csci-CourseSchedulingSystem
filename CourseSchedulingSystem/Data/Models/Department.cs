using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class Department
    {
        private string _code;

        private string _name;

        public Department()
        {
        }

        public Department(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public Guid Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Code
        {
            get => _code;
            set => _code = value.Trim().ToUpper();
        }

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value.Trim();
                NormalizedName = _name.ToUpper();
            }
        }

        public string NormalizedName { get; private set; }

        public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any department has the same code
                if (await context.Departments
                    .Where(d => d.Id != Id)
                    .Where(d => d.Code == Code)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A department already exists with the code {Code}."));

                // Check if any department has the same name
                if (await context.Departments
                    .Where(d => d.Id != Id)
                    .Where(d => d.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A department already exists with the name {Name}."));
            });
        }
    }
}