#!/bin/bash
# Run DispatchR Benchmarks

echo "======================================"
echo "DispatchR vs MediatR Benchmark Runner"
echo "======================================"
echo ""

# Check if running in Release mode
if [ "$1" != "-c" ] && [ "$1" != "--configuration" ]; then
    echo "??  Warning: Running benchmarks in Debug mode is not recommended!"
    echo "   Use: ./run-benchmarks.sh -c Release"
    echo ""
fi

# Navigate to benchmark project
cd "$(dirname "$0")"

echo "Available benchmark options:"
echo "1. Interactive mode (recommended)"
echo "2. All benchmarks"
echo "3. Command benchmarks only"
echo "4. Query benchmarks only"
echo "5. Allocation benchmarks only"
echo "6. Cold start benchmarks only"
echo ""

read -p "Select an option (1-6): " choice

case $choice in
    1)
        echo "Running in interactive mode..."
        dotnet run -c Release
        ;;
    2)
        echo "Running all benchmarks..."
        dotnet run -c Release -- --all
        ;;
    3)
        echo "Running command benchmarks..."
        dotnet run -c Release -- command
        ;;
    4)
        echo "Running query benchmarks..."
        dotnet run -c Release -- query
        ;;
    5)
        echo "Running allocation benchmarks..."
        dotnet run -c Release -- allocation
        ;;
    6)
        echo "Running cold start benchmarks..."
        dotnet run -c Release -- coldstart
        ;;
    *)
        echo "Invalid option. Running in interactive mode..."
        dotnet run -c Release
        ;;
esac

echo ""
echo "Benchmark complete! Check BenchmarkDotNet.Artifacts/results/ for detailed reports."
