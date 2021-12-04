using System.Linq;
using Autofac.Extensions.DependencyInjection;
using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Test.IntegrationTests.Helpers
{
    public class IntegrationTestApplicationFactory<TTestStartup> : WebApplicationFactory<TTestStartup> where TTestStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var host = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHost(builder =>
                {
                    builder.UseStartup<TTestStartup>();
                    builder.UseEnvironment("Development");
                    builder.ConfigureServices(services =>
                    {
                        var databaseContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                        if (databaseContextDescriptor is not null)
                        {
                            services.Remove(databaseContextDescriptor);
                        }

                        services.AddDbContext<DatabaseContext>(options =>
                        {
                            options.UseInMemoryDatabase("Integration_Test_Database");
                        });
                    }).UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
                });

            return host;
        }
    }
}