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
public class QueryBenchmark
{
    private IServiceProvider _dispatchRServiceProvider = null!;
    private IServiceProvider _mediatRServiceProvider = null!;
    private IDispatchR _dispatcher = null!;
    private IMediator _mediator = null!;

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
    public async Task<string> DispatchR_Query()
    {
        return await _dispatcher.Query(new QueryDispatchR(42));
    }

    [Benchmark]
    public async Task<string> MediatR_Query()
    {
        return await _mediator.Send(new QueryMediatR(42));
    }
}
