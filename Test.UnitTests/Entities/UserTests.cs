using Common.Enum;
using Domain.Entities;
using Extensions;
using FluentAssertions;
using Test.UnitTests.TestData;
using Xunit;

namespace Test.UnitTests.Entities
{
    public class UserTests
    {
        [Theory]
        [MemberData(nameof(UserTestData.GetDataForFullName), MemberType = typeof(UserTestData))]
        public void VerifyFullName(User user)
        {
            user.Should().NotNull();
            user.FullName.Should().Be($"{user.FirstName} {user.LastName}");
        }

        [Theory]
        [MemberData(nameof(UserTestData.GetDataForUserRole), MemberType = typeof(UserTestData))]
        public void UserVerifyRole(RoleType roleType, string expected)
        {
            var user = new User
            {
                RoleType = roleType
            };

            user.UserRole.Should().Be(expected);
        }
    }
}
