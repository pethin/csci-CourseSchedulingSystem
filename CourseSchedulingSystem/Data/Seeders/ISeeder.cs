using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Seeders
{
    /// <summary>
    /// Classes that implement this interface in this namespace will be added as a seeder
    /// in the application seed sub-command.
    /// </summary>
    internal interface ISeeder
    {
        Task SeedAsync();
    }
}