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
public class ColdStartBenchmark
{
    [Benchmark(Baseline = true)]
    public async Task DispatchR_ColdStart()
    {
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.RegisterAssembly(Assembly.GetExecutingAssembly());
        });
        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = serviceProvider.GetRequiredService<IDispatchR>();
        
        await dispatcher.Send(new SimpleDispatchRCommand(42));
        
        if (serviceProvider is IDisposable disposable)
            disposable.Dispose();
    }

    [Benchmark]
    public async Task MediatR_ColdStart()
    {
        var services = new ServiceCollection();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        await mediator.Send(new SimpleMediatRCommand(42));
        
        if (serviceProvider is IDisposable disposable)
            disposable.Dispose();
    }
}
