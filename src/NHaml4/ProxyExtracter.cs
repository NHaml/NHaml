using System;
using System.Linq;
using System.Collections.Generic;

namespace NHaml4
{
    public static class ProxyExtracter
    {
        public static Type GetNonProxiedType(Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
                return GetGenericType(ref type);
            else if (IsDynamic(type))
                return GetNonDynamicType(type);
            else
                return type;
        }

        private static Type GetNonDynamicType(Type type)
        {
            var baseType = type.BaseType;
            if (baseType == typeof(object))
            {
                var interfaces = type.GetInterfaces();
                if (interfaces.Length <= 1)
                    throw new Exception(string.Format("Could not create non dynamic type for '{0}'.", type.FullName));
                return GetNonProxiedType(interfaces[0]);
            }
            return GetNonProxiedType(baseType);
        }

        private static Type GetGenericType(ref Type type)
        {
            var genericArguments = type.GetGenericArguments().Select(x => GetNonProxiedType(x));
            type = GetNonProxiedType(type.GetGenericTypeDefinition());
            return type.MakeGenericType(genericArguments.ToArray());
        }

        //HACK: must be a better way of working this out
        private static bool IsDynamic(Type type)
        {
            try
            {
                var location = type.Assembly.Location;
                return false;
            }
            catch (NotSupportedException)
            {
                return true;
            }
        }
    }
}