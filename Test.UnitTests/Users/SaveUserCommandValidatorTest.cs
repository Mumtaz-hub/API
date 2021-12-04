using System.Threading.Tasks;
using Commands.Users;
using Common.Interface;
using Extensions;
using FluentAssertions;
using NSubstitute;

using Test.UnitTests.TestData;
using Xunit;

namespace Test.UnitTests.Users
{

    public class SaveUserCommandValidatorTest : MockDbContextBuilder
    {
        private readonly SaveUserCommandValidator sut;
        private ILoggedOnUserProvider user;

        public SaveUserCommandValidatorTest()
        {
            var context = GetDbContext();
            user = Substitute.For<ILoggedOnUserProvider>();
            sut = new SaveUserCommandValidator(context);
        }
        [Fact]
        public void PasswordTest()
        {
            var password = "123456789$";

            var result1 = password.ToPasswordHmacSha512Hash();
            var hashedPassword1 = result1.hashedPassword;
            var key1 = result1.passwordKey;


            var result2 = password.ToPasswordHmacSha512Hash();
            var hashedPassword2 = result2.hashedPassword;
            var key2 = result2.passwordKey;


            var compareResult1 = password.ToPasswordHmacSha512Hash(key1);
            var shouldResult = compareResult1.IsEqual(hashedPassword1);
            shouldResult.Should().BeTrue();


            var compareResult2 = password.ToPasswordHmacSha512Hash(key2);
            var shouldResult1 = compareResult2.IsEqual(hashedPassword2);
            shouldResult1.Should().BeTrue();

            var compareResult3 = password.ToPasswordHmacSha512Hash(key1);
            var shouldResult3 = compareResult3.IsEqual(hashedPassword2);
            shouldResult3.Should().BeFalse();
        }


        [Fact]
        public async Task FirstNameIsRequired()
        {
            // Arrange
            var command = UserTestData.GetSaveUserCommandTestData;
            command.FirstName = string.Empty;

            // Act
            var result = await sut.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "First Name is required");
        }

        [Fact]
        public async Task LastNameIsRequired()
        {
            // Arrange
            var command = UserTestData.GetSaveUserCommandTestData;
            command.LastName = string.Empty;

            // Act
            var result = await sut.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Last Name is required");
        }
    }
}
