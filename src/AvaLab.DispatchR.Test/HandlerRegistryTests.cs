using AvaLab.DispatchR.Abstraction;
using AvaLab.DispatchR.MicrosoftExtensionsDI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using System.Reflection.Emit;

namespace AvaLab.DispatchR.Test
{
    public class HandlerRegistryTests
    {
        [Fact]
        public void AddDispatchR_WithConfiguration_ShouldRegisterHandlersFromAssemblies()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = typeof(ValidHandlers.TestCommandHandler).Assembly;

            // Act
            services.AddDispatchR(cfg =>
            {
                cfg.RegisterAssembly(assembly);
            });

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var commandHandler = serviceProvider.GetService<ICommandHandler<ValidHandlers.TestCommand>>();
            var commandWithResponseHandler = serviceProvider.GetService<ICommandHandler<ValidHandlers.TestCommandWithResponse, int>>();
            var queryHandler = serviceProvider.GetService<IQueryHandler<ValidHandlers.TestQuery, string>>();

            Assert.NotNull(commandHandler);
            Assert.NotNull(commandWithResponseHandler);
            Assert.NotNull(queryHandler);
            Assert.IsType<ValidHandlers.TestCommandHandler>(commandHandler);
            Assert.IsType<ValidHandlers.TestCommandWithResponseHandler>(commandWithResponseHandler);
            Assert.IsType<ValidHandlers.TestQueryHandler>(queryHandler);
        }

        [Fact]
        public void AddDispatchR_WithConfigurationObject_ShouldRegisterHandlersFromAssemblies()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = typeof(ValidHandlers.TestCommandHandler).Assembly;
            var configuration = new DispatchRServiceConfiguration();
            configuration.RegisterAssembly(assembly);

            // Act
            services.AddDispatchR(configuration);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var commandHandler = serviceProvider.GetService<ICommandHandler<ValidHandlers.TestCommand>>();
            
            Assert.NotNull(commandHandler);
            Assert.IsType<ValidHandlers.TestCommandHandler>(commandHandler);
        }

        [Fact]
        public void AddDispatchR_WithMultipleAssemblies_ShouldRegisterHandlersFromAllAssemblies()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = typeof(ValidHandlers.TestCommandHandler).Assembly;

            // Act
            services.AddDispatchR(cfg =>
            {
                cfg.RegisterAssemblies(assembly);
            });

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var commandHandler = serviceProvider.GetService<ICommandHandler<ValidHandlers.TestCommand>>();
            var queryHandler = serviceProvider.GetService<IQueryHandler<ValidHandlers.TestQuery, string>>();

            Assert.NotNull(commandHandler);
            Assert.NotNull(queryHandler);
        }

        [Fact]
        public void RegisterHandlersFromAssemblies_WhenDuplicateHandlerExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var services = new ServiceCollection();
            
            // Create a dynamic assembly with duplicate handlers to avoid polluting the test assembly
            var assemblyName = new AssemblyName("DuplicateHandlerTestAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            // Define the command type
            var commandTypeBuilder = moduleBuilder.DefineType(
                "DuplicateTestCommand",
                TypeAttributes.Public,
                null,
                new[] { typeof(ICommand) });
            var commandType = commandTypeBuilder.CreateType();

            // Define first handler
            var handler1TypeBuilder = moduleBuilder.DefineType(
                "Handler1",
                TypeAttributes.Public);
            var handler1InterfaceType = typeof(ICommandHandler<>).MakeGenericType(commandType!);
            handler1TypeBuilder.AddInterfaceImplementation(handler1InterfaceType);
            
            var handleMethod1 = handler1TypeBuilder.DefineMethod(
                "Handle",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(Task),
                new[] { commandType, typeof(CancellationToken) });
            var ilGen1 = handleMethod1.GetILGenerator();
            ilGen1.Emit(OpCodes.Call, typeof(Task).GetProperty("CompletedTask")!.GetGetMethod()!);
            ilGen1.Emit(OpCodes.Ret);
            
            var handler1Type = handler1TypeBuilder.CreateType();

            // Define second handler (duplicate)
            var handler2TypeBuilder = moduleBuilder.DefineType(
                "Handler2",
                TypeAttributes.Public);
            handler2TypeBuilder.AddInterfaceImplementation(handler1InterfaceType);
            
            var handleMethod2 = handler2TypeBuilder.DefineMethod(
                "Handle",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(Task),
                new[] { commandType, typeof(CancellationToken) });
            var ilGen2 = handleMethod2.GetILGenerator();
            ilGen2.Emit(OpCodes.Call, typeof(Task).GetProperty("CompletedTask")!.GetGetMethod()!);
            ilGen2.Emit(OpCodes.Ret);
            
            var handler2Type = handler2TypeBuilder.CreateType();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                services.AddDispatchR(cfg =>
                {
                    cfg.RegisterAssembly(assemblyBuilder);
                });
            });

            Assert.Contains("Multiple handlers detected", exception.Message);
            Assert.Contains("Only one handler per command/query is allowed", exception.Message);
        }

        [Fact]
        public void RegisterHandlersFromAssemblies_ShouldRegisterHandlersAsTransient()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = typeof(ValidHandlers.TestCommandHandler).Assembly;

            // Act
            services.AddDispatchR(cfg =>
            {
                cfg.RegisterAssembly(assembly);
            });

            // Assert
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICommandHandler<ValidHandlers.TestCommand>));
            Assert.NotNull(descriptor);
            Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        }

        [Fact]
        public void RegisterHandlersFromAssemblies_ShouldOnlyRegisterSupportedHandlerTypes()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = typeof(ValidHandlers.TestCommandHandler).Assembly;

            // Act
            services.AddDispatchR(cfg =>
            {
                cfg.RegisterAssembly(assembly);
            });

            // Assert - Non-handler interfaces should not be registered
            var nonHandlerService = services.FirstOrDefault(d => d.ServiceType == typeof(ValidHandlers.INonHandlerInterface));
            Assert.Null(nonHandlerService);
        }

        [Fact]
        public void RegisterHandlersFromAssemblies_WithEmptyAssemblyList_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var exception = Record.Exception(() =>
            {
                services.AddDispatchR(cfg =>
                {
                    // Don't register any assemblies
                });
            });

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void RegisterHandlersFromAssemblies_ShouldResolveHandlersCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = typeof(ValidHandlers.TestCommandHandler).Assembly;

            services.AddDispatchR(cfg =>
            {
                cfg.RegisterAssembly(assembly);
            });

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var handler1 = serviceProvider.GetService<ICommandHandler<ValidHandlers.TestCommand>>();
            var handler2 = serviceProvider.GetService<ICommandHandler<ValidHandlers.TestCommand>>();

            // Assert - Should create new instances (transient)
            Assert.NotNull(handler1);
            Assert.NotNull(handler2);
            Assert.NotSame(handler1, handler2);
        }
    }
}

// Separate namespace with valid handlers only (no duplicates)
namespace AvaLab.DispatchR.Test.ValidHandlers
{
    using AvaLab.DispatchR.Abstraction;

    // Test commands and queries
    public class TestCommand : ICommand { }
    public class TestCommandWithResponse : ICommand<int> { }
    public class TestQuery : IQuery<string> { }

    // Test handlers
    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task Handle(TestCommand command, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }

    public class TestCommandWithResponseHandler : ICommandHandler<TestCommandWithResponse, int>
    {
        public Task<int> Handle(TestCommandWithResponse command, CancellationToken ct)
        {
            return Task.FromResult(42);
        }
    }

    public class TestQueryHandler : IQueryHandler<TestQuery, string>
    {
        public Task<string> Handle(TestQuery query, CancellationToken ct)
        {
            return Task.FromResult("test result");
        }
    }

    // Non-handler interface to verify it's not registered
    public interface INonHandlerInterface { }
    public class NonHandlerClass : INonHandlerInterface { }
}
