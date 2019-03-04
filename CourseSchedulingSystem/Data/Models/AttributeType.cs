using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class AttributeType
    {
        public AttributeType()
        {
        }

        public AttributeType(string name)
        {
            Name = name;
        }

        private string _name;

        public Guid Id { get; set; }

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

        public virtual ICollection<CourseAttributeType> CourseAttributeTypes { get; set; }

        public async Task<IEnumerable<ValidationResult>> ValidateAsync(DbContext context)
        {
            var errors = new List<ValidationResult>();

            if (await context.Set<Subject>().AnyAsync(at => at.NormalizedName == NormalizedName))
            {
                errors.Add(new ValidationResult($"An attribute type already exists with the name {Name}.", new[] { "Name" }));
            }

            return errors;
        }
    }
}