using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class Term
    {
        private string _name;

        public Term()
        {
        }

        public Term(string name)
        {
            Name = name;
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

        [NotMapped]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate => TermParts?.OrderBy(tp => tp.StartDate).Select(tp => tp.StartDate).FirstOrDefault();

        [NotMapped]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate => TermParts?.OrderByDescending(tp => tp.EndDate).Select(tp => tp.EndDate).FirstOrDefault();

        [Required]
        [Display(Name = "Archived")]
        public bool IsArchived { get; set; }

        public List<TermPart> TermParts { get; set; }

        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any term has the same name
                if (await context.Terms
                    .Where(t => t.Id != Id)
                    .Where(t => t.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(new ValidationResult($"A term already exists with the name {Name}."));
            });
        }
    }
}