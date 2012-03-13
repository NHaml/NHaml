using System;
using System.Collections.Generic;

namespace NHaml4
{
    public static class ProxyExtracter
    {
        public static Type GetNonProxiedType(Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericArguments = new List<Type>();
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    genericArguments.Add(GetNonProxiedType(genericArgument));
                }
                type = GetNonProxiedType(type.GetGenericTypeDefinition());
                return type.MakeGenericType(genericArguments.ToArray());
            }

            if (IsDynamic(type))
            {
                var baseType = type.BaseType;
                if (baseType == typeof(object))
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Length > 1)
                    {
                        return GetNonProxiedType(interfaces[0]);
                    }
                    throw new Exception(string.Format("Could not create non dynamic type for '{0}'.", type.FullName));
                }
                return GetNonProxiedType(baseType);
            }
            return type;
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