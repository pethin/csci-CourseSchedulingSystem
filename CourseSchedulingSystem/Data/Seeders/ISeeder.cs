using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Seeders
{
    internal interface ISeeder
    {
        Task SeedAsync();
    }
}