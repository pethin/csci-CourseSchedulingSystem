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
        public Subject()
        {
        }

        public Subject(string code, string name)
        {
            Code = code;
            Name = name;
        }

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
            var existingSubjects = await context.Subjects
                .Where(d => d.Code == Code || d.NormalizedName == NormalizedName)
                .ToListAsync();

            if (existingSubjects.Any(d => d.Code == Code))
            {
                errors.Add(new ValidationResult($"A subject already exists with the code {Code}.", new[] { "Code" }));
            }

            if (existingSubjects.Any(d => d.NormalizedName == NormalizedName))
            {
                errors.Add(new ValidationResult($"A subject already exists with the name {Name}.", new[] { "Name" }));
            }

            return errors;
        }
    }
}
