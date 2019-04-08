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
        private static readonly List<Building> BuildingsTemplate = new List<Building>
        {
            new Building("CARR", "Carroll Hall", true),
            new Building("INTR", "Via Internet", true),
            new Building("JOHN", "Johnson Hall", true),
            new Building("OWEN", "Owens Hall", true),
            new Building("RUTL", "Rutledge Building", true),
            new Building("THUR", "Thurmond Hall", true)
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
            var buildingCodes = BuildingsTemplate.Select(m => m.Code).ToHashSet();
            var createdBuildings = _context.Building
                .Where(m => buildingCodes.Contains(m.Code))
                .Select(m => m.Code)
                .ToHashSet();

            var buildings = BuildingsTemplate.Where(b => !createdBuildings.Contains(b.Code));
            foreach (var building in buildings)
                _context.Building.Add(building);

            await _context.SaveChangesAsync();
        }
    }
}