using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Queries.User;
using ViewModel.Users;
using Xunit;

namespace Test.UnitTests.Users
{
    public class UserQueryHandlerTests
    {
        private readonly Faker faker;

        public UserQueryHandlerTests()
        {
            faker = new Faker();
        }
        

        [Fact]
        public async Task UserQueryHandler_ShouldReturnUser_WhenExist()
        {
            //Arrange
            const long id = 1;
            var userViewModel = new UserViewModel
            {
                Id = 1,
                FirstName = faker.Name.FirstName(Name.Gender.Male),
                EmailAddress = faker.Internet.Email()
            };

            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<UserQuery>()).Returns(userViewModel);

            //Act
            var response = await mediator.Send(new UserQuery(id));

            //Assert
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(userViewModel);
        }

        [Fact]
        public async Task UserQueryHandler_ShouldReturnNothing_WhenNotExist()
        {
            const int id = 1;

            //Arrange
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<int>()).ReturnsNull();

            //Act
            var response = await mediator.Send(new UserQuery(id));

            //Assert
            response.Should().BeNull();
        }
    }
}
