using AvaLab.DispatchR.MicrosoftExtensionsDI;
using System.Reflection;

namespace AvaLab.DispatchR.Test
{
    public class DispatchRServiceConfigurationTests
    {
        [Fact]
        public void RegisterAssembly_ShouldAddAssemblyToList()
        {
            // Arrange
            var config = new DispatchRServiceConfiguration();
            var assembly = Assembly.GetExecutingAssembly();

            // Act
            var result = config.RegisterAssembly(assembly);

            // Assert
            Assert.Single(config.HandlersAssemblies);
            Assert.Contains(assembly, config.HandlersAssemblies);
            Assert.Same(config, result); // Fluent API should return same instance
        }

        [Fact]
        public void RegisterAssembly_WhenAssemblyAlreadyRegistered_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var config = new DispatchRServiceConfiguration();
            var assembly = Assembly.GetExecutingAssembly();
            config.RegisterAssembly(assembly);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                config.RegisterAssembly(assembly));

            Assert.Contains("already registered", exception.Message);
            Assert.Contains(assembly.FullName, exception.Message);
        }

        [Fact]
        public void RegisterAssemblies_ShouldAddMultipleAssembliesToList()
        {
            // Arrange
            var config = new DispatchRServiceConfiguration();
            var assembly1 = Assembly.GetExecutingAssembly();
            var assembly2 = typeof(object).Assembly; // mscorlib/System.Private.CoreLib

            // Act
            var result = config.RegisterAssemblies(assembly1, assembly2);

            // Assert
            Assert.Equal(2, config.HandlersAssemblies.Count);
            Assert.Contains(assembly1, config.HandlersAssemblies);
            Assert.Contains(assembly2, config.HandlersAssemblies);
            Assert.Same(config, result); // Fluent API should return same instance
        }

        [Fact]
        public void RegisterAssemblies_WhenAnyAssemblyAlreadyRegistered_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var config = new DispatchRServiceConfiguration();
            var assembly1 = Assembly.GetExecutingAssembly();
            var assembly2 = typeof(object).Assembly;
            config.RegisterAssembly(assembly1);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                config.RegisterAssemblies(assembly1, assembly2));

            Assert.Contains("already registered", exception.Message);
            Assert.Contains(assembly1.FullName, exception.Message);
        }

        [Fact]
        public void RegisterAssemblies_WithNoAssemblies_ShouldNotAddAnything()
        {
            // Arrange
            var config = new DispatchRServiceConfiguration();

            // Act
            var result = config.RegisterAssemblies();

            // Assert
            Assert.Empty(config.HandlersAssemblies);
            Assert.Same(config, result);
        }

        [Fact]
        public void HandlersAssemblies_ShouldBeInitializedAsEmptyList()
        {
            // Arrange & Act
            var config = new DispatchRServiceConfiguration();

            // Assert
            Assert.NotNull(config.HandlersAssemblies);
            Assert.Empty(config.HandlersAssemblies);
        }

        [Fact]
        public void RegisterAssembly_FluentAPI_ShouldAllowChaining()
        {
            // Arrange
            var config = new DispatchRServiceConfiguration();
            var assembly1 = Assembly.GetExecutingAssembly();
            var assembly2 = typeof(object).Assembly;

            // Act
            var result = config
                .RegisterAssembly(assembly1)
                .RegisterAssembly(assembly2);

            // Assert
            Assert.Equal(2, config.HandlersAssemblies.Count);
            Assert.Same(config, result);
        }
    }
}
