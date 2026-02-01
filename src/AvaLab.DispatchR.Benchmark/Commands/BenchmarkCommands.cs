using AvaLab.DispatchR.Abstraction;
using MediatR;

namespace AvaLab.DispatchR.Benchmark.Commands;

// DispatchR Commands
public record SimpleDispatchRCommand(int Value) : ICommand;

public record CommandWithResponseDispatchR(int Value) : ICommand<int>;

public record QueryDispatchR(int Value) : IQuery<string>;

// MediatR Commands
public record SimpleMediatRCommand(int Value) : IRequest;

public record CommandWithResponseMediatR(int Value) : IRequest<int>;

public record QueryMediatR(int Value) : IRequest<string>;
