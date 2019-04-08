using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Data.Seeders
{
    public class SchedulesSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;

        public SchedulesSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await Task.Yield();
        }
    }
}