# DispatchR vs MediatR Benchmarks

This project contains comprehensive performance benchmarks comparing **DispatchR** with **MediatR**.

## Benchmark Categories

### 1. Command Benchmark
Compares the performance of:
- Simple commands (no response)
- Commands with responses

### 2. Query Benchmark
Compares query execution performance between DispatchR and MediatR.

### 3. Allocation Benchmark
Tests memory allocation patterns with varying iteration counts (1, 10, 100):
- Multiple command executions
- Multiple query executions

### 4. Cold Start Benchmark
Measures the overhead of:
- Service container setup
- Handler registration
- First command execution

## Running the Benchmarks

### Interactive Mode (Recommended)
```bash
dotnet run -c Release
```
This will present an interactive menu to choose which benchmarks to run.

### Run All Benchmarks
```bash
dotnet run -c Release -- --all
```

### Run Specific Benchmarks
```bash
# Command benchmarks only
dotnet run -c Release -- command

# Query benchmarks only
dotnet run -c Release -- query

# Allocation benchmarks only
dotnet run -c Release -- allocation

# Cold start benchmarks only
dotnet run -c Release -- coldstart
```

### Run with BenchmarkDotNet Filters
```bash
# Run all benchmarks with a specific filter
dotnet run -c Release -- --filter *DispatchR*

# Run with specific configuration
dotnet run -c Release -- --job short
```

## Understanding Results

The benchmarks provide several metrics:

- **Mean**: Average execution time
- **Median**: Middle value of all measurements
- **Min/Max**: Fastest and slowest execution times
- **Allocated**: Memory allocated per operation
- **Rank**: Performance ranking (1 = best)
- **Ratio**: Performance relative to baseline (lower is better)

## Important Notes

1. **Always run in Release mode** (`-c Release`) for accurate results
2. Close unnecessary applications to minimize interference
3. Benchmarks may take several minutes to complete
4. Results are saved in `BenchmarkDotNet.Artifacts` directory
5. The baseline for comparison is DispatchR

## Requirements

- .NET 10.0 SDK
- BenchmarkDotNet 0.14.0
- MediatR 12.4.1

## Output

Results are generated in multiple formats:
- Console output with summary tables
- HTML reports in `BenchmarkDotNet.Artifacts/results/`
- Markdown reports for documentation
- CSV files for further analysis

## Example Output

```
BenchmarkDotNet v0.14.0
// ** Summary **

|                    Method |      Mean |     Error |    StdDev | Ratio | Rank | Allocated |
|-------------------------- |----------:|----------:|----------:|------:|-----:|----------:|
| DispatchR_SimpleCommand   |  1.234 ?s | 0.012 ?s | 0.011 ?s |  1.00 |    1 |     512 B |
|   MediatR_SimpleCommand   |  1.456 ?s | 0.015 ?s | 0.014 ?s |  1.18 |    2 |     768 B |
```

## Tips for Best Results

1. Run benchmarks on a quiet system (no heavy background tasks)
2. Use a dedicated benchmark session (close IDE, browsers, etc.)
3. Run multiple times to verify consistency
4. Check the `BenchmarkDotNet.Artifacts` folder for detailed reports
5. Use the `--job long` parameter for more accurate results (takes longer)

## Contributing

When adding new benchmarks:
1. Create a new class in the `Benchmarks` folder
2. Add `[MemoryDiagnoser]` and `[RankColumn]` attributes
3. Follow the naming convention: `[Feature]Benchmark`
4. Ensure both DispatchR and MediatR scenarios are included
5. Mark one method as `[Benchmark(Baseline = true)]`
