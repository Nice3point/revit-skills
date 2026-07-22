---
name: revit-benchmarking
description: >
  Design, write, run, or review BenchmarkDotNet measurements that run inside Autodesk Revit with Nice3point.BenchmarkDotNet.Revit.
  USE FOR: comparing viable Revit API implementations or measuring a Revit hot path, including the runner's WithCurrentConfiguration requirement and the OnGlobalSetup/OnGlobalCleanup document lifecycle.
  DO NOT USE FOR: benchmarking .NET code that does not call the Revit API.
license: MIT
---

# Revit Benchmarking

A Revit benchmark runs inside Revit and answers one production decision about a Revit API hot path.
`Nice3point.BenchmarkDotNet.Revit` marshals the benchmark onto Revit's thread; running it needs a matching licensed Revit installation.

## When to use

- Choosing between viable Revit API implementations on measured evidence.
- Measuring a Revit hot path on a representative model.

## When not to use

- The measured operation does not call the Revit API.

## Workflow

### Step 1: Write the benchmark class

Derive from `RevitApiBenchmark`.
Open the model in `OnGlobalSetup` and close it in `OnGlobalCleanup` — the base binds `[GlobalSetup]`/`[GlobalCleanup]` and calls these overrides; never add those attributes yourself.
Each `[Benchmark]` holds one compared operation and returns its result; a returned result keeps the JIT from eliminating it. Declare the alternatives as sibling `[Benchmark]` methods in the same class.

A small application-level comparison needs no document:

```csharp
public class XyzBenchmarks : RevitApiBenchmark
{
    [Benchmark]
    public XYZ Constructor()
    {
        return new XYZ(3, 4, 5);
    }

    [Benchmark]
    public XYZ Factory()
    {
        return Application.Create.NewXYZ(3, 4, 5);
    }
}
```

A benchmark that needs a seeded model opens it once in setup; keep the seeding out of the measured method:

```csharp
public class CollectorBenchmarks : RevitApiBenchmark
{
    private Document _document = null!;

    protected sealed override void OnGlobalSetup()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using var transaction = new Transaction(_document, "Seed model");
        transaction.Start();
        var level = Level.Create(_document, 0);
        for (var i = 0; i < 1000; i++)
        {
            Wall.Create(_document, Line.CreateBound(new XYZ(i, 0, 0), new XYZ(i + 1, 0, 0)), level.Id, false);
        }
        transaction.Commit();
    }

    protected sealed override void OnGlobalCleanup()
    {
        _document.Close(false);
    }

    [Benchmark]
    public IList<Element> ToElements()
    {
        return new FilteredElementCollector(_document)
            .WhereElementIsNotElementType()
            .ToElements();
    }

    [Benchmark]
    public List<Element> ToList()
    {
        return new FilteredElementCollector(_document)
            .WhereElementIsNotElementType()
            .ToList();
    }
}
```

### Step 2: Configure the runner with the current build configuration

BenchmarkDotNet builds in `Release` by default, which fails for Revit's multi-version configurations.
Apply `WithCurrentConfiguration()` to the job; it then builds the active `Release.RNN`.

```csharp
var configuration = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.Default.WithCurrentConfiguration())
    .AddDiagnoser(MemoryDiagnoser.Default);

BenchmarkRunner.Run<CollectorBenchmarks>(configuration);
```

### Step 3: Run and decide

```shell
dotnet run -c Release.RNN
```

`RNN` is the target Revit-year configuration, for example `Release.R26`; it must match the licensed Revit installed on the machine.
Iterate with a dry run first, then measure the final comparison on a quiet machine.
Read the Markdown report; compare time, allocation, and output correctness, and record why the chosen implementation applies to production.

## Validation

- [ ] The benchmark derives from `RevitApiBenchmark`.
- [ ] The document is opened in `OnGlobalSetup` and closed in `OnGlobalCleanup`, outside the measured method.
- [ ] The runner job uses `WithCurrentConfiguration()`.
- [ ] The selected `Release.RNN` configuration matches the installed Revit runtime.
- [ ] The result drives a concrete implementation decision.

## Common Pitfalls

| Pitfall                                                                   | Correct approach                                                                                                      |
|---------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------|
| Runner uses the default `Release` job                                     | Add `Job.Default.WithCurrentConfiguration()`.                                                                         |
| Seeding the model inside a `[Benchmark]` method                           | Do it in `OnGlobalSetup`.                                                                                             |
| `[GlobalSetup]`/`[GlobalCleanup]` added directly                          | Override `OnGlobalSetup`/`OnGlobalCleanup`; the base binds them.                                                      |
| Discovery features like `[Params]`/`[ParamsAllValues]` using a Revit type | BenchmarkDotNet reads them during discovery, before Revit initializes, and throws; use primitives or non-Revit types. |
| `RevitApiBenchmark` not found                                             | The `Nice3point.BenchmarkDotNet.Revit` package is not referenced.                                                     |
