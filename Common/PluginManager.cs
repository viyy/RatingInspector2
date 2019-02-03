using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Common
{
    public static class PluginManager
    {
        public static List<TInterface> GetPlugins<TInterface>()
        {
            return (from file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"plugins\"),
                    "*.dll")
                from type in Assembly.LoadFrom(file).GetTypes()
                where type.GetInterfaces().Contains(typeof(TInterface))
                select (TInterface) Activator.CreateInstance(type)).ToList();
        }
    }
}