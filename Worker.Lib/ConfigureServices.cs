using Commands;
using Data;
using Hangfire;
using Hangfire.MediatR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace Worker.Lib
{
    public static class ConfigureServices
    {
        public static void AddWorker(this IServiceCollection services)
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
        }
    }
}
