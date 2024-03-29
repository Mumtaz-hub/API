﻿using System;
using System.Linq;
using System.Reflection;

namespace Extensions
{
    public static class ReflectionExtensions
    {
        public static bool HasAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute
        {
            return method.GetCustomAttributes(typeof(TAttribute), false).Any();
        }
    }
}
