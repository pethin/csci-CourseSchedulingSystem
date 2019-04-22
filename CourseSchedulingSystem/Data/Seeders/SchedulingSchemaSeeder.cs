using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Data.Seeders
{
    /// <summary>
    /// Seeds instructional methods and meeting types.
    /// </summary>
    public class SchedulingSchemaSeeder : ISeeder
    {
        /// <summary>
        ///     Template for creating instructional methods.
        /// </summary>
        private static readonly List<InstructionalMethod> InstructionalMethods = new List<InstructionalMethod>
        {
            new InstructionalMethod(Guid.Parse("00000000-0000-0000-0000-000000000001"), "CLASS", "Classroom", true),
            new InstructionalMethod(Guid.Parse("00000000-0000-0000-0000-000000000002"), "HYB01", "Hybrid: 1-24% taught online", true),
            new InstructionalMethod(Guid.Parse("00000000-0000-0000-0000-000000000003"), "HYB25", "Hybrid: 25-49% taught online", true),
            new InstructionalMethod(Guid.Parse("00000000-0000-0000-0000-000000000004"), "HYB50", "Hybrid: 50-74% taught online", true),
            new InstructionalMethod(Guid.Parse("00000000-0000-0000-0000-000000000005"), "HYB75", "Hybrid: 75-99% taught online", true),
            new InstructionalMethod(Guid.Parse("00000000-0000-0000-0000-000000000006"), "WEB", "Online", false)
        };
        
        /// <summary>
        ///     Template for creating meeting types.
        /// </summary>
        private static readonly List<MeetingType> MeetingTypes = new List<MeetingType>
        {
            MeetingType.ClassMeetingType,
            MeetingType.AdditionalClassTimeMeetingType
        };

        private readonly ApplicationDbContext _context;

        public SchedulingSchemaSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedInstructionalMethodsAsync();
            await SeedMeetingTypesAsync();
        }

        private async Task SeedInstructionalMethodsAsync()
        {
            var templateIds = InstructionalMethods.Select(t => t.Id).ToHashSet();
            var createdTemplatesIds = _context.InstructionalMethods
                .Where(m => templateIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToHashSet();

            var instructionalMethods = InstructionalMethods.Where(t => !createdTemplatesIds.Contains(t.Id)).ToList();
            _context.InstructionalMethods.AddRange(instructionalMethods);

            await _context.SaveChangesAsync();
        }
        
        private async Task SeedMeetingTypesAsync()
        {
            var templateIds = MeetingTypes.Select(t => t.Id).ToHashSet();
            var createdTemplatesIds = _context.MeetingTypes
                .Where(m => templateIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToHashSet();

            var meetingTypes = MeetingTypes.Where(t => !createdTemplatesIds.Contains(t.Id)).ToList();
            _context.MeetingTypes.AddRange(meetingTypes);

            await _context.SaveChangesAsync();
        }
    }
}