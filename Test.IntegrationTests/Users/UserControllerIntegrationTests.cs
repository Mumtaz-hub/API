using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Api;
using Bogus;
using Bogus.DataSets;
using Commands.Users;
using Common.Enum;
using FluentAssertions;
using Test.IntegrationTests.Constants;
using Test.IntegrationTests.Extensions;
using Test.IntegrationTests.Helpers;
using Test.IntegrationTests.Providers;
using ViewModel.Users;
using Xunit;

namespace Test.IntegrationTests.Users
{
    public class UserControllerIntegrationTests : IntegrationTestBase
    {
        public UserControllerIntegrationTests(IntegrationTestApplicationFactory<Startup> factory) : base(factory)
        {
        }


        [Fact]
        public async Task Save_User_Return_Success()
        {
            // Arrange
            var faker = new Faker();
            var password = $"{faker.Internet.Password(prefix: "TestPassword12@")}$"; //to satisfy the requirement for passwords

            var mockData = new Faker<SaveUserCommand>()
                .RuleFor(u => u.FirstName, f => f.Name.LastName(Name.Gender.Male))
                .RuleFor(u => u.LastName, f => f.Name.LastName(Name.Gender.Male))
                .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.MobileNumber, f => f.Phone.PhoneNumber("##########"))
                .RuleFor(u => u.Password, password)
                .RuleFor(u => u.ConfirmPassword, password)
                .RuleFor(u => u.RoleType, f => f.PickRandom<RoleType>());

            var command = mockData.Generate();
            var client = Factory.CreateClient();

            // Act
            var id = await client.PostAsync<SaveUserCommand, long>(ApiRouteConstants.Users.PostUser, command);

            // Assert
            id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_User_WhenUser_Exists()
        {
            // Arrange
            var faker = new Faker();
            var password = $"{faker.Internet.Password(prefix: "TestPassword12@")}$"; //to satisfy the requirement for passwords

            var mockData = new Faker<SaveUserCommand>()
                .RuleFor(u => u.FirstName, f => f.Name.LastName(Name.Gender.Male))
                .RuleFor(u => u.LastName, f => f.Name.LastName(Name.Gender.Male))
                .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.MobileNumber, f => f.Phone.PhoneNumber("##########"))
                .RuleFor(u => u.Password, password)
                .RuleFor(u => u.ConfirmPassword, password)
                .RuleFor(u => u.RoleType, f => f.PickRandom<RoleType>());

            var command = mockData.Generate();
            var claimsProvider = TestClaimsProvider.WithSuperAdminClaims();
            var client = Factory.CreateClientWithTestAuth(claimsProvider);
            var id = await client.PostAsync<SaveUserCommand, long>(ApiRouteConstants.Users.PostUser, command);

            // Act
            var response = await client.GetAsync<UserViewModel>(ApiRouteConstants.Users.GetUserByIdRoute.Replace("{id}", id.ToString()));

            // Assert
            response.FirstName.ShouldEqual($"{command.FirstName}");
            response.EmailAddress.ShouldEqual(command.EmailAddress);
            response.MobileNumber.ShouldEqual(command.MobileNumber);
        }

        [Fact]
        public async Task GetAll_Users_ReturnsEmptyResponse()
        {
            //Arrange 
            var claimsProvider = TestClaimsProvider.WithAdminClaims();
            var client = Factory.CreateClientWithTestAuth(claimsProvider);

            // Act
            var response = await client.GetAsync<List<UserViewModel>>(ApiRouteConstants.Users.GetUsers);

            //Assert
            response.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetAll_Users_Returns_Unauthorized()
        {
            //Arrange 
         
            var client = Factory.CreateClient();

            // Act
            var userOneOf = await client.GetAsyncOneOf<List<UserViewModel>>(ApiRouteConstants.Users.GetUsers);
           
            userOneOf.Switch(
                _ =>
                {
                  
                }, response =>
                {
                    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
                });
        }

        [Fact]
        public async Task Get_User__Returns_Unauthorized()
        {
            //Arrange 
            const int id = 1;
            var client = Factory.CreateClient();

            // Act
           
            var userOneOf = await client.GetAsyncOneOf<UserViewModel>(ApiRouteConstants.Users.GetUserByIdRoute.Replace("{id}", id.ToString()));
            userOneOf.Switch(
                _ =>
                {
                   
                }, response =>
                {
                    //Assert
                    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
                });
        }
    }
}