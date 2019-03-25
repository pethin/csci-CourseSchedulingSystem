using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class SchedulingSchemaSeeder : ISeeder
    {
        /// <summary>
        ///     Template for creating instructional methods.
        /// </summary>
        private static readonly List<InstructionalMethod> InstructionalMethodsTemplate = new List<InstructionalMethod>
        {
            new InstructionalMethod("Classroom", true),
            new InstructionalMethod("Hybrid", true),
            new InstructionalMethod("Hybrid: 1-24% taught online", true),
            new InstructionalMethod("Hybrid: 25-49% taught online", true),
            new InstructionalMethod("Hybrid: 50-74% taught online", true),
            new InstructionalMethod("Hybrid: 75-99% taught online", true),
            new InstructionalMethod("Online", false),
        };

        private readonly ApplicationDbContext _context;

        public SchedulingSchemaSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedInstructionalMethodsAsync();
        }

        private async Task SeedInstructionalMethodsAsync()
        {
            var instructionalMethodNames = InstructionalMethodsTemplate.Select(im => im.Name).ToHashSet();
            var createdInstructionalMethods = _context.InstructionalMethods
                .Where(im => instructionalMethodNames.Contains(im.Name))
                .Select(im => im.Name)
                .ToHashSet();

            var instructionalMethods = InstructionalMethodsTemplate.Where(imt => !createdInstructionalMethods.Contains(imt.Name));
            foreach (var instructionalMethod in instructionalMethods) await _context.InstructionalMethods.AddAsync(instructionalMethod);

            await _context.SaveChangesAsync();
        }
    }
}