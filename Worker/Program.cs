using Commands;
using Data;
using Hangfire;
using Hangfire.MediatR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
 
namespace Worker
{
    public class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    var assembliesContainingMediatrHandlers = new[]
                    {
                        typeof(Command<>).Assembly
                    };
                    services.AddMediatR(assembliesContainingMediatrHandlers);

                    services.AddDbContext<DatabaseContext>();
                    services.AddHangfire(configuration =>
                    {
                        configuration.UseSqlServerStorage("Server=.\\;Database=Hangfire_Test;User Id=sa;Password=xts@12345;Integrated Security=false;");
                        configuration.UseMediatR();
                    });

                    services.AddHangfireServer();

                    //services.AddHostedService<ProgramHostedService>();
                });
    }
}
