using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Behaviors.Authorization.Extensions;
using Api.Caching.Extensions;
using Api.Extensions;
using Api.Infrastructure;
using Api.Redis.PublishSubscribe.Extensions;
using Api.SignalR;
using Api.SignalR.Extensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Commands.Email.Templates;
using Commands.Extensions;
using Commands.Login;
using Common;
using Data;
using Data.Extensions;
using ElmahCore.Mvc;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.MediatR;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Oauth;
using Oauth.Extensions;
using Serilog;
using Queries.Extensions;

namespace Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public ILifetimeScope ApplicationContainer { get; private set; }

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.overrides.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
            Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddOptions();

            services.ConfigureElmah();

            services.AddAutoMapperWithProfiles();

            services.AddControllers();

            services.AddJwtAuthentication(Configuration);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            // Add framework services.  
            services.AddMvc(options =>
            {
                //By the type  
                options.Filters.Add(typeof(CommandHydratorAttribute));
            }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginCommandValidator>());

            //services.AddMediatorAuthorization();

            services.ConfigureDependecyInjectionForQueryPipelineBehaviours();

            services.AddCachingBehavior(Configuration);

            services.AddSignalRDependency();

            services.AddConnectionMultiplexerDependency(Configuration);

            services.AddSwaggerGenNewtonsoftSupport();

            services.AddSwaggerSetup();

            services.AddAppSettings(Configuration);

            services.AddEmailSettings(Configuration);

            services.AddMediatrWithHandlers();

            services.ConfigureCommonServices();

            services.ConfigureDependecyInjectionForCommandPipelineBehaviours();


            services.AddSingleton(Configuration);

            ConfigureDatabase(services);

            // add logging
            services.AddLogging();
            services.AddHangfire(configuration =>
            {
                configuration.UseSqlServerStorage(Configuration.GetConnectionString("Hangfire"));
                configuration.UseMediatR();
            });

            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ApplicationContainer = app.ApplicationServices.GetAutofacRoot();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var database = serviceScope.ServiceProvider.GetService<DatabaseContext>();
                if (database is not null && !database.Database.IsInMemory() && !database.AllMigrationsApplied())
                {
                    database.Database.Migrate();
                    database.EnsureSeeded();
                }
            }

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true) // allow any origin
                .AllowCredentials());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseRouting();

            ConfigureAuth(app);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
                //{
                //    Authorization = new[] { new HangFireAuthFilter() }
                //});
                endpoints.MapProjectHub();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthFilter() }
            });
            //app.UseHangfireServer();
            app.ConfigureElmahStyleSheet();
            app.UseElmah();
            app.UseSwaggerSetup();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<EmailTemplates>();
        }

        protected virtual void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnnectionString"),
                builder =>
                {
                    builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(20), new List<int>());
                }));
        }

        protected void ConfigureAuth(IApplicationBuilder app)
        {
            var tokenEndpoint = Configuration["Oauth:TokenEndpoint"];
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Oauth:SecretKey"]));
            var issuer = Configuration["Oauth:Issuer"];
            var audienceId = Configuration["Oauth:AudienceId"];
            var tokenExpirationInDays = Convert.ToInt64(Configuration["Oauth:AccessTokenExpirationInDays"]);

            app.UseSimpleTokenProvider(new TokenProviderOptions
            {
                Path = tokenEndpoint,
                Audience = audienceId,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                Expiration = TimeSpan.FromDays(tokenExpirationInDays),
                IdentityResolver = GetIdentity,
            });

            app.UseAuthentication();
        }

        private async Task<Tuple<ClaimsIdentity, Result>> GetIdentity(string username, string password)
        {
            var mediator = ApplicationContainer.Resolve<IMediator>();
            var result = await mediator.Send(new LoginCommand
            {
                UserName = username,
                Password = password
            });

            var claim = new ClaimsIdentity(result.Value);
            return new Tuple<ClaimsIdentity, Result>(claim, result);
        }
    }
}
