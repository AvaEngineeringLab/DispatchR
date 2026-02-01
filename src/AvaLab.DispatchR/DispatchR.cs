using AvaLab.DispatchR.Abstraction;

namespace AvaLab.DispatchR
{
    public class DispatchR(IServiceProvider serviceProvider) : IDispatchR
    {
        public Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(query);

            var queryType = query.GetType();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
            dynamic handler = serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException($"No query handler registered for {queryType.Name} -> {typeof(TResponse).Name}");

            return handler.Handle((dynamic)query, ct);
        }

        public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
            dynamic handler = serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException($"No command handler registered for {commandType.Name} -> {typeof(TResponse).Name}");

            return handler.Handle((dynamic)command, ct);
        }

        public Task Send(ICommand command, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            dynamic handler = serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException($"No command handler registered for {commandType.Name} -> void");

            return handler.Handle((dynamic)command, ct);
        }
    }
}