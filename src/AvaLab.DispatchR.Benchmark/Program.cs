using BenchmarkDotNet.Running;
using AvaLab.DispatchR.Benchmark.Benchmarks;

namespace AvaLab.DispatchR.Benchmark;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== DispatchR vs MediatR Benchmarks ===\n");
        Console.WriteLine("Running comprehensive performance benchmarks comparing DispatchR and MediatR");
        Console.WriteLine("This may take several minutes...\n");

        if (args.Length > 0 && args[0] == "--all")
        {
            // Run all benchmarks
            BenchmarkRunner.Run<CommandBenchmark>();
            BenchmarkRunner.Run<QueryBenchmark>();
            BenchmarkRunner.Run<AllocationBenchmark>();
            BenchmarkRunner.Run<ColdStartBenchmark>();
        }
        else if (args.Length > 0)
        {
            // Run specific benchmark
            switch (args[0].ToLower())
            {
                case "command":
                    BenchmarkRunner.Run<CommandBenchmark>();
                    break;
                case "query":
                    BenchmarkRunner.Run<QueryBenchmark>();
                    break;
                case "allocation":
                    BenchmarkRunner.Run<AllocationBenchmark>();
                    break;
                case "coldstart":
                    BenchmarkRunner.Run<ColdStartBenchmark>();
                    break;
                default:
                    ShowUsage();
                    break;
            }
        }
        else
        {
            // Run summary by default
            var summary = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run -c Release                  # Interactive mode");
        Console.WriteLine("  dotnet run -c Release -- --all         # Run all benchmarks");
        Console.WriteLine("  dotnet run -c Release -- command       # Command benchmarks only");
        Console.WriteLine("  dotnet run -c Release -- query         # Query benchmarks only");
        Console.WriteLine("  dotnet run -c Release -- allocation    # Allocation benchmarks only");
        Console.WriteLine("  dotnet run -c Release -- coldstart     # Cold start benchmarks only");
    }
}
