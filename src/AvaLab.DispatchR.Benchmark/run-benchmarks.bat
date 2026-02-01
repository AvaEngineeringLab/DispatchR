@echo off
REM Run DispatchR Benchmarks

echo ======================================
echo DispatchR vs MediatR Benchmark Runner
echo ======================================
echo.

REM Navigate to benchmark project
cd /d "%~dp0"

echo Available benchmark options:
echo 1. Interactive mode (recommended)
echo 2. All benchmarks
echo 3. Command benchmarks only
echo 4. Query benchmarks only
echo 5. Allocation benchmarks only
echo 6. Cold start benchmarks only
echo.

set /p choice="Select an option (1-6): "

if "%choice%"=="1" (
    echo Running in interactive mode...
    dotnet run -c Release
) else if "%choice%"=="2" (
    echo Running all benchmarks...
    dotnet run -c Release -- --all
) else if "%choice%"=="3" (
    echo Running command benchmarks...
    dotnet run -c Release -- command
) else if "%choice%"=="4" (
    echo Running query benchmarks...
    dotnet run -c Release -- query
) else if "%choice%"=="5" (
    echo Running allocation benchmarks...
    dotnet run -c Release -- allocation
) else if "%choice%"=="6" (
    echo Running cold start benchmarks...
    dotnet run -c Release -- coldstart
) else (
    echo Invalid option. Running in interactive mode...
    dotnet run -c Release
)

echo.
echo Benchmark complete! Check BenchmarkDotNet.Artifacts\results\ for detailed reports.
pause
