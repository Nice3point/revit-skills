# Revit Benchmark Template

**Load when:** measuring Revit API code with BenchmarkDotNet inside Revit.

`revit-benchmark` creates an executable benchmark project.
It references `Nice3point.BenchmarkDotNet.Revit`, BenchmarkDotNet, and `Nice3point.Revit.Api.RevitAPI`.
The generated `Program.cs` runs the starter benchmark for the specified configuration.

```shell
dotnet new revit-benchmark --name MyBenchmarks
dotnet run -c Release.R26 -- --filter '*'
```

The template has no options.
Restrict setup and assertions to what the measurement requires.
Place product functionality in a `revit-addin-module` project, then reference the code being measured as appropriate.

## Validation

- [ ] The project builds for the selected `Debug.RNN` or `Release.RNN` configuration.
- [ ] Benchmark code measures Revit API work; application startup and unrelated test setup are excluded from the measurement.
