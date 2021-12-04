using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions
{
    public static class AutoMapperExtensions
    {
        public static void AddAutoMapperWithProfiles(this IServiceCollection services)
        {
            if (services == null) 
                throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(GetAutoMapperProfilesFromAllAssemblies().ToArray());

        }

        public static IEnumerable<Type> GetAutoMapperProfilesFromAllAssemblies()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies() 
                from aType in assembly.GetTypes() where aType.IsClass && !aType.IsAbstract && aType.IsSubclassOf(typeof(Profile)) 
                select aType;
        }
    }
}
