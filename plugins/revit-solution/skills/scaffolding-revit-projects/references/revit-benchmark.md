# Revit Benchmark Template

**Load when:** measuring Revit API code with BenchmarkDotNet inside Revit.

`revit-benchmark` creates an executable benchmark project.
It references `Nice3point.BenchmarkDotNet.Revit`, BenchmarkDotNet, and `Nice3point.Revit.Api.RevitAPI`.
The generated `Program.cs` runs the starter benchmark for the specified configuration.

```shell
dotnet new revit-benchmark --name MyBenchmarks
```

The template has no options.
Keep setup and assertions focused on reliable measurement.
Keep product functionality in a `revit-addin-module` project, then reference the code being measured as appropriate.

## Validation

- [ ] The project builds for the selected `Debug.RNN` or `Release.RNN` configuration.
- [ ] Benchmark code measures Revit API work rather than application startup or unrelated test setup.
