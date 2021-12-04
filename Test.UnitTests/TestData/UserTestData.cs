using System.Collections.Generic;
using Bogus;
using Bogus.DataSets;
using Commands.Users;
using Common.Enum;
using Domain.Entities;

namespace Test.UnitTests.TestData
{
    public static class UserTestData
    {
        public static SaveUserCommand GetSaveUserCommandTestData
        {
            get
            {
                var faker = new Faker();
                var password = $"{faker.Internet.Password()}$"; //to satisfy the requirement for passwords to have at least 1 special character

                var mockData = new Faker<SaveUserCommand>()
                    .RuleFor(u => u.FirstName, f => f.Name.LastName(Name.Gender.Male))
                    .RuleFor(u => u.LastName, f => f.Name.LastName(Name.Gender.Male))
                    .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                    .RuleFor(u => u.Password, password)
                    .RuleFor(u => u.ConfirmPassword, password)
                    .RuleFor(u => u.RoleType, f => f.PickRandom<RoleType>());

                return mockData.Generate();
            }
        }

        public static IEnumerable<object[]> GetDataForFullName()
        {
            Faker faker = new();
            yield return new object[]
            {
                new User {FirstName = faker.Name.FirstName(Name.Gender.Male), LastName = faker.Name.LastName(Name.Gender.Male)}
            };
            yield return new object[]
            {
                new User {FirstName = faker.Name.FirstName(Name.Gender.Male), LastName = faker.Name.LastName(Name.Gender.Male)}
            };
            yield return new object[]
            {
                new User {FirstName = faker.Name.FirstName(Name.Gender.Female), LastName = faker.Name.LastName(Name.Gender.Female)}
            };
            yield return new object[]
            {
                new User {FirstName = faker.Name.FirstName(Name.Gender.Female), LastName = faker.Name.LastName(Name.Gender.Female)}
            };
        }

        public static IEnumerable<object[]> GetDataForUserRole()
        {
            yield return new object[] { RoleType.SuperAdmin, "Super Admin" };
            yield return new object[] { RoleType.Admin, "Admin" };
            yield return new object[] { RoleType.Leader, "Leader" };
            yield return new object[] { RoleType.Participant, "Participant" };
            yield return new object[] { RoleType.Sponsors, "Sponsors" };
            yield return new object[] { RoleType.Developer, "Developer" };
            yield return new object[] { RoleType.Manager, "Manager" };
            yield return new object[] { RoleType.Ceo, "CEO" };
        }
    }
}
