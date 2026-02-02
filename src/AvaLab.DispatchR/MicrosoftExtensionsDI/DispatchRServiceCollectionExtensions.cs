using AvaLab.DispatchR.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaLab.DispatchR.MicrosoftExtensionsDI
{
    public static class DispatchRServiceCollectionExtensions
    {
        public static IServiceCollection AddDispatchR(this IServiceCollection services, Action<DispatchRServiceConfiguration> configuration)
        {
            DispatchRServiceConfiguration cfg = new();
            configuration(cfg);
            return services.AddDispatchR(cfg);
        }

        public static IServiceCollection AddDispatchR(this IServiceCollection services, DispatchRServiceConfiguration configuration)
        {
            // Register the dispatcher
            services.AddSingleton<IDispatchR, DispatchR>();

            // Register handlers from assemblies
            return HandlerRegistry.RegisterHandlersFromAsemblies(services, configuration.HandlersAssemblies);
        }
    }
}