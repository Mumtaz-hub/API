using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions
{
    public static class JwtAuthenticationExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfigurationRoot configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));


            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Oauth:SecretKey"]));
            var issuer = configuration["Oauth:Issuer"];
            var audienceId = configuration["Oauth:AudienceId"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    //cfg.RequireHttpsMetadata = false;
                    //cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = audienceId,

                        // Validate the token expiry
                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        // If you want to allow a certain amount of clock drift, set that here:
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
    }
}
