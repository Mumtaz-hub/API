using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Oauth
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate next;
        private readonly TokenProviderOptions options;
        private readonly ILogger logger;
        private readonly JsonSerializerSettings serializerSettings;

        public TokenProviderMiddleware(RequestDelegate next, IOptions<TokenProviderOptions> options, ILogger logger)
        {
            this.next = next;
            this.options = options.Value;
            this.logger = logger;

            ThrowIfInvalidOptions(this.options);

            serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public Task Invoke(HttpContext context)
        {
            //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            //context.Response.Headers.Add("Access-Control-Allow-Headers", "authorization,Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name");
            //context.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(options.Path, StringComparison.Ordinal))
            {
                return next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            logger.Information("Handling request: " + context.Request.Path);
            return GenerateToken(context);
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];
            var timezone = context.Request.Form["timezone"].FirstOrDefault();

            var (claimsIdentity, result) = await options.IdentityResolver(username, password);

            if (result.IsFailure)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(result.HtmlFormattedFailures);
                return;
            }

            var now = DateTime.UtcNow;

            // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimConstants.TimeZone, timezone?? "India Standard Time"),
                new Claim(JwtRegisteredClaimNames.Jti, await options.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
            }.Concat(claimsIdentity.Claims);

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
             options.Issuer,
             options.Audience,
             claims,
             now,
             now.Add(options.Expiration),
             options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)options.Expiration.TotalSeconds
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "authorization,Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, serializerSettings));
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Path));
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            if (options.IdentityResolver == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.IdentityResolver));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }
        }

        /// <summary>
        /// Get this datetime as a Unix epoch timestamp (seconds since Jan 1, 1970, midnight UTC).
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>Seconds since Unix epoch.</returns>
        public static long ToUnixEpochDate(DateTime date) => new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
    }
}
