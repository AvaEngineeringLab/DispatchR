using AvaLab.DispatchR.Abstraction;
using AvaLab.DispatchR.Benchmark.Commands;
using AvaLab.DispatchR.MicrosoftExtensionsDI;
using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AvaLab.DispatchR.Benchmark.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class AllocationBenchmark
{
    private IServiceProvider _dispatchRServiceProvider = null!;
    private IServiceProvider _mediatRServiceProvider = null!;
    private IDispatchR _dispatcher = null!;
    private IMediator _mediator = null!;

    [Params(1, 10, 100)]
    public int IterationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Setup DispatchR
        var dispatchRServices = new ServiceCollection();
        dispatchRServices.AddDispatchR(cfg =>
        {
            cfg.RegisterAssembly(Assembly.GetExecutingAssembly());
        });
        _dispatchRServiceProvider = dispatchRServices.BuildServiceProvider();
        _dispatcher = _dispatchRServiceProvider.GetRequiredService<IDispatchR>();

        // Setup MediatR
        var mediatRServices = new ServiceCollection();
        mediatRServices.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        _mediatRServiceProvider = mediatRServices.BuildServiceProvider();
        _mediator = _mediatRServiceProvider.GetRequiredService<IMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (_dispatchRServiceProvider is IDisposable dispatchRDisposable)
            dispatchRDisposable.Dispose();
        
        if (_mediatRServiceProvider is IDisposable mediatRDisposable)
            mediatRDisposable.Dispose();
    }

    [Benchmark(Baseline = true)]
    public async Task DispatchR_MultipleCommands()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            await _dispatcher.Send(new SimpleDispatchRCommand(i));
        }
    }

    [Benchmark]
    public async Task MediatR_MultipleCommands()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            await _mediator.Send(new SimpleMediatRCommand(i));
        }
    }

    [Benchmark]
    public async Task DispatchR_MultipleQueries()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            await _dispatcher.Query(new QueryDispatchR(i));
        }
    }

    [Benchmark]
    public async Task MediatR_MultipleQueries()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            await _mediator.Send(new QueryMediatR(i));
        }
    }
}
