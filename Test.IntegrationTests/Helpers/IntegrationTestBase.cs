using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Test.IntegrationTests.Helpers
{
    public abstract class IntegrationTestBase : IClassFixture<IntegrationTestApplicationFactory<Startup>>
    {
        protected WebApplicationFactory<Startup> Factory { get; }

        protected IntegrationTestBase(WebApplicationFactory<Startup> factory)
        {
            Factory = factory;
        }

 
        // Add you other helper methods here
    }
}