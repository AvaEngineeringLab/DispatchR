using AvaLab.DispatchR.Abstraction;
using NSubstitute;

namespace AvaLab.DispatchR.Test
{
    public class DispatchRTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DispatchR _dispatcher;

        public DispatchRTests()
        {
            _serviceProvider = Substitute.For<IServiceProvider>();
            _dispatcher = new DispatchR(_serviceProvider);
        }

        #region Query Tests

        [Fact]
        public async Task Query_WithValidQuery_ShouldCallHandlerAndReturnResponse()
        {
            // Arrange
            var query = new TestQuery();
            var expectedResponse = "test response";
            var handler = Substitute.For<IQueryHandler<TestQuery, string>>();
            handler.Handle(query, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expectedResponse));

            _serviceProvider.GetService(typeof(IQueryHandler<TestQuery, string>)).Returns(handler);

            // Act
            var result = await _dispatcher.Query(query);

            // Assert
            Assert.Equal(expectedResponse, result);
            await handler.Received(1).Handle(query, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Query_WithCancellationToken_ShouldPassTokenToHandler()
        {
            // Arrange
            var query = new TestQuery();
            var expectedResponse = "test response";
            var cts = new CancellationTokenSource();
            var handler = Substitute.For<IQueryHandler<TestQuery, string>>();
            handler.Handle(query, cts.Token).Returns(Task.FromResult(expectedResponse));

            _serviceProvider.GetService(typeof(IQueryHandler<TestQuery, string>)).Returns(handler);

            // Act
            var result = await _dispatcher.Query(query, cts.Token);

            // Assert
            Assert.Equal(expectedResponse, result);
            await handler.Received(1).Handle(query, cts.Token);
        }

        [Fact]
        public async Task Query_WhenNullQuery_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _dispatcher.Query<string>(null!));
        }

        [Fact]
        public async Task Query_WhenHandlerNotRegistered_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var query = new TestQuery();
            _serviceProvider.GetService(typeof(IQueryHandler<TestQuery, string>)).Returns((object?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _dispatcher.Query(query));

            Assert.Contains("No query handler registered", exception.Message);
            Assert.Contains(nameof(TestQuery), exception.Message);
        }

        #endregion Query Tests

        #region Command Tests (with response)

        [Fact]
        public async Task Send_WithValidCommandAndResponse_ShouldCallHandlerAndReturnResponse()
        {
            // Arrange
            var command = new TestCommandWithResponse();
            var expectedResponse = 42;
            var handler = Substitute.For<ICommandHandler<TestCommandWithResponse, int>>();
            handler.Handle(command, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expectedResponse));

            _serviceProvider.GetService(typeof(ICommandHandler<TestCommandWithResponse, int>)).Returns(handler);

            // Act
            var result = await _dispatcher.Send(command);

            // Assert
            Assert.Equal(expectedResponse, result);
            await handler.Received(1).Handle(command, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Send_WithCommandAndResponseAndCancellationToken_ShouldPassTokenToHandler()
        {
            // Arrange
            var command = new TestCommandWithResponse();
            var expectedResponse = 42;
            var cts = new CancellationTokenSource();
            var handler = Substitute.For<ICommandHandler<TestCommandWithResponse, int>>();
            handler.Handle(command, cts.Token).Returns(Task.FromResult(expectedResponse));

            _serviceProvider.GetService(typeof(ICommandHandler<TestCommandWithResponse, int>)).Returns(handler);

            // Act
            var result = await _dispatcher.Send(command, cts.Token);

            // Assert
            Assert.Equal(expectedResponse, result);
            await handler.Received(1).Handle(command, cts.Token);
        }

        [Fact]
        public async Task Send_WithNullCommandAndResponse_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _dispatcher.Send<int>(null!));
        }

        [Fact]
        public async Task Send_WhenCommandWithResponseHandlerNotRegistered_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var command = new TestCommandWithResponse();
            _serviceProvider.GetService(typeof(ICommandHandler<TestCommandWithResponse, int>)).Returns((object?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _dispatcher.Send(command));

            Assert.Contains("No command handler registered", exception.Message);
            Assert.Contains(nameof(TestCommandWithResponse), exception.Message);
        }

        #endregion Command Tests (with response)

        #region Command Tests (without response)

        [Fact]
        public async Task Send_WithValidCommandNoResponse_ShouldCallHandler()
        {
            // Arrange
            var command = new TestCommand();
            var handler = Substitute.For<ICommandHandler<TestCommand>>();
            handler.Handle(command, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            _serviceProvider.GetService(typeof(ICommandHandler<TestCommand>)).Returns(handler);

            // Act
            await _dispatcher.Send(command);

            // Assert
            await handler.Received(1).Handle(command, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Send_WithCommandNoResponseAndCancellationToken_ShouldPassTokenToHandler()
        {
            // Arrange
            var command = new TestCommand();
            var cts = new CancellationTokenSource();
            var handler = Substitute.For<ICommandHandler<TestCommand>>();
            handler.Handle(command, cts.Token).Returns(Task.CompletedTask);

            _serviceProvider.GetService(typeof(ICommandHandler<TestCommand>)).Returns(handler);

            // Act
            await _dispatcher.Send(command, cts.Token);

            // Assert
            await handler.Received(1).Handle(command, cts.Token);
        }

        [Fact]
        public async Task Send_WithNullCommandNoResponse_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _dispatcher.Send((ICommand)null!));
        }

        [Fact]
        public async Task Send_WhenCommandNoResponseHandlerNotRegistered_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var command = new TestCommand();
            _serviceProvider.GetService(typeof(ICommandHandler<TestCommand>)).Returns((object?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _dispatcher.Send(command));

            Assert.Contains("No command handler registered", exception.Message);
            Assert.Contains(nameof(TestCommand), exception.Message);
        }

        #endregion Command Tests (without response)

        #region Test Classes

        public class TestQuery : IQuery<string>
        { }

        public class TestCommand : ICommand
        { }

        public class TestCommandWithResponse : ICommand<int>
        { }

        #endregion Test Classes
    }
}