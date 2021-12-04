using System;
using ElmahCore;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions
{
    public static class ElmahCoreExtensions
    {
        private static readonly string ElmahPath;
        private static readonly string ElmahLogFilePath;

        static ElmahCoreExtensions()
        {
            ElmahPath = @"errors";
            ElmahLogFilePath = "./logs";
        }


        public static void ConfigureElmah(this IServiceCollection services)
        {
            services.AddElmah<XmlFileErrorLog>(options =>
            {
                //options.CheckPermissionAction = context => context.User.Identity.IsAuthenticated;
                options.Path = ElmahPath;
                options.LogPath = ElmahLogFilePath;
            });
        }


        public static void ConfigureElmahStyleSheet(this IApplicationBuilder app)
        {
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/" + ElmahPath, StringComparison.OrdinalIgnoreCase), appBuilder =>
             {
                 appBuilder.Use(next =>
                 {
                     return async ctx =>
                     {
                         ctx.Features.Get<IHttpBodyControlFeature>().AllowSynchronousIO = true;

                         await next(ctx);
                     };
                 });
             });
        }
    }
}
