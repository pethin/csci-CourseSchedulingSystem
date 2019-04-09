using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class ScheduleType
    {
        private string _code;
        private string _name;

        public ScheduleType()
        {
        }

        public ScheduleType(Guid id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }

        public Guid Id { get; set; }
        
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Code
        {
            get => _code;
            set => _code = value?.Trim().ToUpperInvariant();
        }

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value?.Trim();
                NormalizedName = _name?.ToUpperInvariant();
            }
        }

        public string NormalizedName { get; private set; }

        public List<CourseScheduleType> CourseScheduleTypes { get; set; }
        
        public List<CourseSection> CourseSections { get; set; }

        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any schedule type has the same code
                if (await context.ScheduleTypes
                    .Where(st => st.Id != Id)
                    .Where(st => st.Code == Code)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A schedule type already exists with the code {Code}."));
                
                // Check if any schedule type has the same name
                if (await context.ScheduleTypes
                    .Where(st => st.Id != Id)
                    .Where(st => st.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A schedule type already exists with the name {Name}."));
            });
        }
    }
}