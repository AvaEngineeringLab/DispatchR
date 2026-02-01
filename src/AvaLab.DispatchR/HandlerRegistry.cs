using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AvaLab.DispatchR
{
    internal class HandlerRegistry
    {
        private static readonly Type[] SupportedHandlerTypes =
        [
            typeof(Abstraction.ICommandHandler<>),
            typeof(Abstraction.ICommandHandler<,>),
            typeof(Abstraction.IQueryHandler<,>)
        ];

        public static IServiceCollection RegisterHandlersFromAsemblies(IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            var registeredHandlers = new HashSet<Type>();

            foreach (var assembly in assemblies)
            {
                RegisterHandlersFromAssembly(services, assembly, registeredHandlers);
            }

            return services;
        }

        private static void RegisterHandlersFromAssembly(IServiceCollection services, Assembly assembly, HashSet<Type> registeredHandlers)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                RegisterHandlerType(services, type, registeredHandlers);
            }
        }

        private static void RegisterHandlerType(IServiceCollection services, Type type, HashSet<Type> registeredHandlers)
        {
            var interfaces = type.GetInterfaces();

            foreach (var iface in interfaces)
            {
                if (IsHandlerInterface(iface))
                {
                    if (!registeredHandlers.Add(iface))
                    {
                        throw new InvalidOperationException(
                            $"Multiple handlers detected for '{iface}'. Only one handler per command/query is allowed.");
                    }

                    services.AddTransient(iface, type);
                }
            }
        }

        private static bool IsHandlerInterface(Type interfaceType)
        {
            if (!interfaceType.IsGenericType)
            {
                return false;
            }

            var genericTypeDef = interfaceType.GetGenericTypeDefinition();

            foreach (var supportedType in SupportedHandlerTypes)
            {
                if (genericTypeDef == supportedType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}