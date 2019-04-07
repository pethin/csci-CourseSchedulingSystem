using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class Building
    {
        private string _name;
        private string _code;

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

        [Required]
        [Display(Name = "Enabled")]
        public bool IsEnabled { get; set; }

        public string NormalizedName { get; private set; }

        public List<Room> Rooms { get; set; }

        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any other building has the same code
                if (await context.Building
                    .Where(bd => bd.Id != Id)
                    .Where(bd => bd.Code == Code)
                    .AnyAsync())
                {
                    await yield.ReturnAsync(
                        new ValidationResult($"A building already exists with the code {Code}."));
                }

                // Check if any other building has the same name
                if (await context.Building
                    .Where(bd => bd.Id != Id)
                    .Where(bd => bd.NormalizedName == NormalizedName)
                    .AnyAsync())
                {
                    await yield.ReturnAsync(
                        new ValidationResult($"A building already exists with the name {Name}."));
                }
            });
        }
    }
}