using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Tests.Factories
{
    public class UserFactory : Faker<ApplicationUser>
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
