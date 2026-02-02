# DispatchR vs MediatR Performance Comparison

## Overview

This benchmark project provides a comprehensive performance comparison between **DispatchR** and **MediatR**, two popular .NET libraries for implementing the mediator pattern and CQRS.

## What We're Measuring

### 1. **Command Benchmark** (`CommandBenchmark.cs`)
- **Simple Commands**: Commands without return values
- **Commands with Response**: Commands that return a value
- **Focus**: Raw execution speed and memory allocation

### 2. **Query Benchmark** (`QueryBenchmark.cs`)
- **Query Execution**: Read operations returning typed results
- **Focus**: Query handling performance and memory efficiency

### 3. **Allocation Benchmark** (`AllocationBenchmark.cs`)
- **Batch Operations**: 1, 10, and 100 iterations
- **Multiple Commands**: Sequential command execution
- **Multiple Queries**: Sequential query execution
- **Focus**: Memory allocation patterns under load

### 4. **Cold Start Benchmark** (`ColdStartBenchmark.cs`)
- **Container Setup**: DI container configuration
- **Handler Registration**: Assembly scanning and registration
- **First Execution**: Initial command dispatch
- **Focus**: Application startup overhead

## Key Differences Between DispatchR and MediatR

### DispatchR
- Lightweight, focused implementation
- Separate `ICommand` and `IQuery` interfaces
- Simple dynamic dispatch mechanism
- Minimal dependencies

### MediatR
- Mature, feature-rich library
- Single `IRequest` interface for all operations
- Pipeline behaviors support
- Notification system
- More extensive feature set

## Expected Results

### Performance
- **DispatchR**: Expected to be faster due to simpler implementation
- **MediatR**: May have slight overhead from additional features

### Memory Allocation
- **DispatchR**: Lower allocations with minimal abstraction
- **MediatR**: Potentially higher allocations due to pipeline infrastructure

### Cold Start
- **DispatchR**: Faster startup due to simpler registration
- **MediatR**: Slightly slower due to more complex initialization

## Running the Benchmarks

### Quick Start
```bash
# Windows
run-benchmarks.bat

# Linux/macOS
chmod +x run-benchmarks.sh
./run-benchmarks.sh
```

### Manual Execution
```bash
# All benchmarks
dotnet run -c Release -- --all

# Specific benchmark
dotnet run -c Release -- command
dotnet run -c Release -- query
dotnet run -c Release -- allocation
dotnet run -c Release -- coldstart
```

## Interpreting Results

### Key Metrics

1. **Mean**: Average execution time across all runs
   - Lower is better
   - Most reliable indicator of typical performance

2. **Allocated**: Memory allocated per operation
   - Lower is better
   - Important for high-throughput scenarios

3. **Ratio**: Performance relative to baseline (DispatchR)
   - 1.00 = baseline (DispatchR)
   - > 1.00 = slower than baseline
   - < 1.00 = faster than baseline

4. **Rank**: Relative performance ranking
   - 1 = best performance
   - Higher numbers indicate slower performance

### Example Interpretation

```
|                    Method |      Mean | Allocated | Ratio | Rank |
|-------------------------- |----------:|----------:|------:|-----:|
| DispatchR_SimpleCommand   |  1.234 ?s |     512 B |  1.00 |    1 |
|   MediatR_SimpleCommand   |  1.456 ?s |     768 B |  1.18 |    2 |
```

This shows:
- DispatchR is ~18% faster (1.00 vs 1.18 ratio)
- DispatchR uses 256 bytes less memory per operation
- DispatchR ranks first in performance

## Recommendations

### Use DispatchR When:
- Performance is critical
- You want minimal overhead
- You prefer explicit command/query separation
- You don't need MediatR's pipeline behaviors

### Use MediatR When:
- You need pipeline behaviors (logging, validation, etc.)
- You want a mature, battle-tested solution
- You need the notification/event system
- Performance difference is acceptable for your use case

## Contributing

To add new benchmarks:

1. Create a new class in `Benchmarks/` folder
2. Inherit attributes: `[MemoryDiagnoser]`, `[RankColumn]`
3. Implement `[GlobalSetup]` and `[GlobalCleanup]`
4. Mark one method as `[Benchmark(Baseline = true)]`
5. Ensure fair comparison between both libraries

## Notes

- Always run benchmarks in **Release** mode
- Close other applications to minimize interference
- Run on a representative production-like environment
- Results may vary based on:
  - CPU architecture
  - Available memory
  - .NET runtime version
  - Operating system

## Results Location

After running, results are saved in:
- `BenchmarkDotNet.Artifacts/results/` - HTML and Markdown reports
- Console output - Summary tables

## Questions?

Check the main DispatchR documentation or open an issue on GitHub.
