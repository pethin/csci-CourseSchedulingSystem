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
        private string _code;
        private string _name;

        public InstructionalMethod()
        {
        }

        public InstructionalMethod(Guid id, string code, string name, bool requiresRoom = true)
        {
            Id = id;
            Code = code;
            Name = name;
            IsRoomRequired = requiresRoom;
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
                // Check if any instructional method has the same code
                if (await context.InstructionalMethods
                    .Where(im => im.Id != Id)
                    .Where(im => im.Code == Code)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"An instructional methods already exists with the code {Code}."));
                
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