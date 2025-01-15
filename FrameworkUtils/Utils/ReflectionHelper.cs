using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;

namespace FrameworkUtils.Utils
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Finds and returns a MethodInfo of a method, based on a parameter: 'full type name' + 'method name'.
        /// NOTE: Can find only *public* methods.
        /// </summary>
        public static MethodInfo GetCodeMethod(string methodPath, bool isStatic = true, bool throwIfNotFound = true)
        {
            if (string.IsNullOrEmpty(methodPath))
                return null;

            MethodInfo result = null;

            int lastDotIndex = methodPath.LastIndexOf('.');
            if (lastDotIndex > 0)
            {
                string className = methodPath.Substring(0, lastDotIndex);
                string methodName = methodPath.Substring(lastDotIndex + 1);
                if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(methodName))
                {
                    Type type = null;

                    try
                    {
                        type = DevExpress.Persistent.Base.ReflectionHelper.GetType(className);
                    }
                    catch (Exception)
                    {
                        if (throwIfNotFound)
                            throw;
                    }

                    if (type != null)
                    {
                        BindingFlags flags = BindingFlags.Public;
                        if (isStatic)
                            flags |= BindingFlags.Static;
                        else
                            flags |= BindingFlags.Instance;

                        result = type.GetMethod(methodName, flags);
                        if (result == null && throwIfNotFound)
                            throw new UserFriendlyException($"Code method: could not find method: '{methodName}' in type: '{className}'");
                    }
                    else if (throwIfNotFound)
                        throw new UserFriendlyException($"Code method: could not find type: '{className}'");
                }
            }

            if (result == null && throwIfNotFound)
                throw new UserFriendlyException($"Method path is invalid: '{methodPath}'");

            return result;
        }

        /// <summary>
        /// Returns a list of classes or interfaces that inherit from the targetType, from the all loaded assembly.
        /// </summary>
        public static IEnumerable<Type> GetImplementingClasses(Type targetType, bool excludeSystemTypes, bool excludeAbstractTypes)
        {
            Guard.ArgumentNotNull(targetType, nameof(targetType));

            List<Type> result = new List<Type>();

            try
            {
                foreach (Assembly assembly in Thread.GetDomain().GetAssemblies())
                {
                    IEnumerable<Type> typesFromAssembly = GetImplementingClasses(assembly, targetType, excludeSystemTypes, excludeAbstractTypes);
                    result.AddRange(typesFromAssembly);
                }
            }
            catch (Exception ex)
            {
                Tracing.Tracer.LogError(ex);
                System.Diagnostics.Debugger.Break();
            }

            return result;
        }

        /// <summary>
        /// Returns a list of classes or interfaces that inherit from the targetType, from the given assembly.
        /// </summary>
        public static IEnumerable<Type> GetImplementingClasses(Assembly assembly, Type targetType, bool excludeSystemTypes, bool excludeAbstractTypes)
        {
            Guard.ArgumentNotNull(assembly, nameof(assembly));
            Guard.ArgumentNotNull(targetType, nameof(targetType));

            List<Type> result = new List<Type>();

            if (excludeSystemTypes &&
              (assembly.FullName.StartsWith("System.") || assembly.FullName.StartsWith("Microsoft.")))
                return result;

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (excludeAbstractTypes && type.IsAbstract)
                    continue;

                if (targetType.IsInterface)
                {
                    if (type.GetInterface(targetType.FullName) != null)
                    {
                        result.Add(type);
                    }
                }
                else if (type.IsSubclassOf(targetType))
                {
                    result.Add(type);
                }
            }

            return result;
        }
    }
}
