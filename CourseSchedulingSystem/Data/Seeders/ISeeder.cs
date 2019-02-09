using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Seeders
{
    interface ISeeder
    {
        Task SeedAsync();
    }
}
