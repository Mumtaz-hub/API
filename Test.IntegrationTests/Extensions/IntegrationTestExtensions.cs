using System.Linq;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;

namespace Test.IntegrationTests.Extensions
{
    public static class IntegrationTestExtensions
    {
        public static void ShouldEqual(this object one, object two, params string[] ignore)
        {
            var compare = new CompareLogic(new ComparisonConfig
            {
                IgnoreObjectTypes = true,
                MembersToIgnore = ignore.ToList(),
                MaxDifferences = int.MaxValue,//show all differences
            });
            var result = compare.Compare(one, two);

            result.AreEqual.Should().BeTrue(result.DifferencesString);
        }
    }
}
