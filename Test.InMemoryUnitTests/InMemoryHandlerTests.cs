using System;
using System.IO;
using System.Threading.Tasks;
using Api.Extensions;
using Commands.Users;
using Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.InMemoryUnitTests
{
    public abstract class InMemoryHandlerTests
    {
        protected IServiceProvider ServiceProvider;

        protected InMemoryHandlerTests()
        {
            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.Development.json", true)
                .AddJsonFile($"appsettings.overrides.json", true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddScoped<DatabaseContext>();

            services.AddAutoMapperWithProfiles();

            services.AddAppSettings(configuration);
            services.AddEmailSettings(configuration);

            services.AddMediatrWithHandlers();
            services.ConfigureCommonServices();

            ServiceProvider = services.BuildServiceProvider();
        }

        protected async Task AssertUserExistsInContext(long id, SaveUserCommand command)
        {
            var context = ServiceProvider.GetRequiredService<DatabaseContext>();

            var user = await context.User
                .FirstOrDefaultAsync(u => u.Id == id
                                          && u.FirstName.Equals(command.FirstName, StringComparison.CurrentCultureIgnoreCase)
                                          && u.LastName.Equals(command.LastName, StringComparison.CurrentCultureIgnoreCase)
                                          && u.EmailAddress.Equals(command.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
                                         );

            user.Should().NotBeNull();
            user.FirstName.Should().Be(command.FirstName);
            user.LastName.Should().Be(command.LastName);
            user.EmailAddress.Should().Be(command.EmailAddress);
          
        }
    }
}
