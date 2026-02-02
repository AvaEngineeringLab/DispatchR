using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AvaLab.DispatchR.MicrosoftExtensionsDI
{
    public class DispatchRServiceConfiguration
    {
        public List<Assembly> HandlersAssemblies { get; } = [];

        public DispatchRServiceConfiguration RegisterAssembly(Assembly assembly)
        {
            if (HandlersAssemblies.Contains(assembly))
            {
                throw new InvalidOperationException($"Assembly '{assembly.FullName}' is already registered.");
            }

            HandlersAssemblies.Add(assembly);

            return this;
        }

        public DispatchRServiceConfiguration RegisterAssemblies(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (HandlersAssemblies.Contains(assembly))
                {
                    throw new InvalidOperationException($"Assembly '{assembly.FullName}' is already registered.");
                }

                HandlersAssemblies.Add(assembly);
            }

            return this;
        }
    }
}

