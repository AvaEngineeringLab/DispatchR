using AvaLab.DispatchR.Abstraction;
using AvaLab.DispatchR.Benchmark.Commands;
using MediatR;

namespace AvaLab.DispatchR.Benchmark.Handlers;

// DispatchR Handlers
public class SimpleDispatchRCommandHandler : ICommandHandler<SimpleDispatchRCommand>
{
    public Task Handle(SimpleDispatchRCommand command, CancellationToken ct)
    {
        var result = command.Value * 2;
        return Task.CompletedTask;
    }
}

public class CommandWithResponseDispatchRHandler : ICommandHandler<CommandWithResponseDispatchR, int>
{
    public Task<int> Handle(CommandWithResponseDispatchR command, CancellationToken ct)
    {
        return Task.FromResult(command.Value * 2);
    }
}

public class QueryDispatchRHandler : IQueryHandler<QueryDispatchR, string>
{
    public Task<string> Handle(QueryDispatchR query, CancellationToken ct)
    {
        return Task.FromResult($"Result: {query.Value}");
    }
}

// MediatR Handlers
public class SimpleMediatRCommandHandler : IRequestHandler<SimpleMediatRCommand>
{
    public Task Handle(SimpleMediatRCommand request, CancellationToken cancellationToken)
    {
        var result = request.Value * 2;
        return Task.CompletedTask;
    }
}

public class CommandWithResponseMediatRHandler : IRequestHandler<CommandWithResponseMediatR, int>
{
    public Task<int> Handle(CommandWithResponseMediatR request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Value * 2);
    }
}

public class QueryMediatRHandler : IRequestHandler<QueryMediatR, string>
{
    public Task<string> Handle(QueryMediatR request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Result: {request.Value}");
    }
}
