using DataCenter.Common;
using DataCenter.MultiKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyWorks.IoC
{
    public static class AssemblyExtensions
    {
        public static Repository<Assembly, List<TypeInfo>> DefinedTypes = new Repository<Assembly, List<TypeInfo>>(
            assembly => assembly.DefinedTypes.Where(t => t.IsClass && !t.IsAbstract).ToList());

        public static MRepository<Type, Assembly, List<TypeInfo>> AssignableTypes =
            new MRepository<Type, Assembly, List<TypeInfo>>(
                (type, assembly) => DefinedTypes[assembly]
                    .Where(t => type.GetTypeInfo().IsAssignableFrom(t))
                    .ToList());

    }
}
