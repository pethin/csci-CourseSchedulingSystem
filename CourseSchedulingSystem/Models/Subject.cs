using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Models
{
    public class Subject
    {
        private string _name;
        private string _code;

        public Guid Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Code
        {
            get => _code;
            set => _code = value.Trim().ToUpper();
        }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
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

        public List<Course> Courses { get; set; }

        public async Task<IEnumerable<ValidationResult>> ValidateAsync(ApplicationDbContext context)
        {
            var errors = new List<ValidationResult>();

            if (await context.Departments.FirstOrDefaultAsync(d => d.Code == Code) != null)
            {
                errors.Add(new ValidationResult($"A subject already exists with the code {Code}.", new[] { "Code" }));
            }

            if (await context.Departments.FirstOrDefaultAsync(d => d.NormalizedName == NormalizedName) != null)
            {
                errors.Add(new ValidationResult($"A subject already exists with the name {Name}.", new[] { "Name" }));
            }

            return errors;
        }
    }
}
