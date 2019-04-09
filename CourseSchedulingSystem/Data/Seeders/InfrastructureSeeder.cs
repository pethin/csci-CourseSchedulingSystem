using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class InfrastructureSeeder : ISeeder
    {
        /// <summary>
        ///     Template for creating buildings.
        /// </summary>
        private static readonly List<Building> Buildings = new List<Building>
        {
            new Building(Guid.Parse("00000000-0000-0000-0000-000000000001"), "CARR", "Carroll Hall", true),
            new Building(Guid.Parse("00000000-0000-0000-0000-000000000002"), "INTR", "Via Internet", true),
            new Building(Guid.Parse("00000000-0000-0000-0000-000000000003"), "JOHN", "Johnson Hall", true),
            new Building(Guid.Parse("00000000-0000-0000-0000-000000000004"), "OWEN", "Owens Hall", true),
            new Building(Guid.Parse("00000000-0000-0000-0000-000000000005"), "RUTL", "Rutledge Building", true),
            new Building(Guid.Parse("00000000-0000-0000-0000-000000000006"), "THUR", "Thurmond Hall", true)
        };

        private readonly ApplicationDbContext _context;

        public InfrastructureSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedBuildingsAsync();
        }

        private async Task SeedBuildingsAsync()
        {
            var templateIds = Buildings.Select(t => t.Id).ToHashSet();
            var createdTemplatesIds = _context.Buildings
                .Where(m => templateIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToHashSet();

            var buildings = Buildings.Where(t => !createdTemplatesIds.Contains(t.Id)).ToList();
            buildings.ForEach(s => s.IsTemplate = true);
            _context.Buildings.AddRange(buildings);

            await _context.SaveChangesAsync();
        }
    }
}