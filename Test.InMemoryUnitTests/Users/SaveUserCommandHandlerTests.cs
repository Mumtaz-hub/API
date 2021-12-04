using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using Bogus.DataSets;
using Commands.Users;
using Common;
using Common.Enum;
using Data;
using Extensions;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.InMemoryUnitTests.Users
{
    public class SaveUserCommandHandlerTests : InMemoryHandlerTests
    {
        private readonly SaveUserCommandHandler saveUserCommandHandler;
        private readonly Faker faker;

        public SaveUserCommandHandlerTests()
        {
            var context = ServiceProvider.GetRequiredService<DatabaseContext>();
            var mediatr = ServiceProvider.GetRequiredService<IMediator>();
            var mapper = ServiceProvider.GetRequiredService<IMapper>();
            var settings = ServiceProvider.GetRequiredService<AppSettings>();
            faker = new Faker();

            saveUserCommandHandler = new SaveUserCommandHandler(context, mapper, mediatr);
        }


        [Fact]
        public async Task SaveUserToDbContext()
        {
            //Arrange
            var handler = saveUserCommandHandler;
            var command = new SaveUserCommand
            {
                FirstName = faker.Name.FirstName(Name.Gender.Male),
                LastName = faker.Name.LastName(Name.Gender.Male),
                EmailAddress = faker.Internet.Email(),
                Password = $"{faker.Internet.Password()}@", //to satisfy the requirement for passwords to have at least 1 special character
                RoleType = faker.PickRandom<RoleType>()
            };

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue(result.Failures.JoinWithComma());
            await AssertUserExistsInContext(result.Value, command);
        }
    }
}