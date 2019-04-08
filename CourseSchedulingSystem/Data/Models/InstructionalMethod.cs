using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class InstructionalMethod
    {
        private string _name;

        public InstructionalMethod()
        {
        }

        public InstructionalMethod(string name, bool requiresRoom = true)
        {
            Name = name;
            IsRoomRequired = requiresRoom;
        }

        public Guid Id { get; set; }

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

        [Required]
        [Display(Name = "Requires Room")]
        public bool IsRoomRequired { get; set; }
        
        public List<CourseSection> CourseSections { get; set; }

        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any instructional method has the same name
                if (await context.InstructionalMethods
                    .Where(im => im.Id != Id)
                    .Where(im => im.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"An instructional methods already exists with the name {Name}."));
            });
        }
    }
}