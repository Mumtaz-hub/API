using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Api.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSwaggerGen(ops =>
            {
                ops.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AspNet Core Api Project",
                    Description = "AspNet Core Project 5.0",
                    Contact = new OpenApiContact
                    {
                        Name = "mumtazuser",
                        Email = "mghata@yahoo.com"
                    }
                });

                ops.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                ops.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void UseSwaggerSetup(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNet Core Api Project");
            });
        }
    }
}
