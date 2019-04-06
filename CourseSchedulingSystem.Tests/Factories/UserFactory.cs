using Bogus;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Tests.Factories
{
    public class UserFactory : Faker<User>
    {
        public UserFactory()
        {
            RuleFor(o => o.UserName, GenerateName);
        }

        private string GenerateName(Faker faker)
        {
            return $"{faker.Name.LastName()}{faker.Name.FirstName()[0]}{faker.Random.Number(1, 9)}".ToLower();
        }
    }
}