---
name: revit-testing
description: >
  Write, run, or review Autodesk Revit API tests that execute inside Revit with Nice3point.TUnit.Revit.
  USE FOR: writing RevitApiTest classes whose bodies run on Revit's single thread via the RevitThreadExecutor, asserting observable model behavior with TUnit, and running the suite against a matching Revit install.
  DO NOT USE FOR: supplying the documents, services, or data cases a test runs against (use revit-test-fixtures), scaffolding the test project (create it from the revit-tunit template), or tests that never call the Revit API.
license: MIT
---

# Revit Testing

Every Revit API call must run on the single thread that initialized Revit.
`Nice3point.TUnit.Revit` marshals each test and hook onto that thread through the `RevitThreadExecutor`; the test body reads like ordinary API code.
It builds on TUnit and Microsoft.Testing.Platform; running the tests needs a matching licensed Revit installation.

It applies to a project scaffolded from the `revit-tunit` template — with the project structure and the assembly-level `RevitThreadExecutor` already configured — and covers writing tests inside it, not setting the project up.

## When to use

- Writing or reviewing a test for your own logic or helpers that call the Revit API.
- Asserting that an operation produced the expected model, file, or value.

## When not to use

- Providing the document, service, or parameterized cases a test consumes — use `revit-test-fixtures`.
- The code under test never touches the Revit API — write a plain TUnit test with no `RevitApiTest` base and no executor.

## Workflow

### Step 1: Write a test on the Revit thread

Inherit `RevitApiTest`; it exposes the shared `Application`.
TUnit constructs a new instance of the test class for every test; instance fields and properties are always isolated between tests — use them freely for per-test state.
Structure the body as distinct Arrange, Act, and Assert blocks, and assert the observable result, not framework plumbing.

```csharp
public sealed class BoundingBoxExtensionsTests : RevitApiTest
{
    [Test]
    public async Task Union_NonOverlappingBoxes_EnclosesBothExtents()
    {
        // Arrange
        var first = new BoundingBoxXYZ { Min = new XYZ(0, 0, 0), Max = new XYZ(1, 1, 1) };
        var second = new BoundingBoxXYZ { Min = new XYZ(2, 2, 2), Max = new XYZ(3, 3, 3) };

        // Act
        var union = first.Union(second);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(union.Min.IsAlmostEqualTo(XYZ.Zero)).IsTrue();
            await Assert.That(union.Max.IsAlmostEqualTo(new XYZ(3, 3, 3))).IsTrue();
        }
    }
}
```

`Assert.Multiple()` groups related checks so one failure does not hide the rest.
TUnit assertions are awaited: `IsEqualTo(...).Within(tol)` for doubles, `IsTrue()`, `IsNotEmpty()`, `.All().Satisfy(...)` for collections, and `.Throws<TException>()` for failures.

### Step 2: Keep every Revit-touching member on the Revit thread

The assembly-level `[assembly: TestExecutor<RevitThreadExecutor>]` (in `TestsConfiguration.cs`) runs every test on Revit's thread; individual tests need no attribute.

- A `[Before]`/`[After]` hook that calls the Revit API carries `[HookExecutor<RevitThreadExecutor>]`.
- A test that must run off the Revit thread overrides with its own `[TestExecutor<OtherExecutor>]`.
- Load Revit API types lazily — a field initializer runs at construction, before Revit is injected:

```csharp
// BAD — runs before Revit exists
private readonly ElementId _levelId = new ElementId(BuiltInCategory.OST_Levels);

// GOOD — resolved on first use, on the Revit thread
private ElementId LevelId => field ??= new ElementId(BuiltInCategory.OST_Levels);
```

Discovery happens before Revit is injected and off its thread: TUnit constructs the test class, evaluates every data source, and resolves every dependency-injection service at discovery.
No constructor, field initializer, data-source member, or injected service may touch the Revit API at construction — defer that work to the test body or a `[Before]` hook that runs on the Revit thread.

### Step 3: Parameterize and supply fixtures

Feed a small fixed set of primitive cases inline with `[Arguments]`; the test body builds the Revit objects.
`[Arguments]`, method data sources, and custom data sources are basic TUnit features; this skill adds the Revit-thread constraints.
For data-source choices beyond the Revit fixtures below, read TUnit's [Method Data Sources source](https://raw.githubusercontent.com/thomhurst/TUnit/main/docs/docs/writing-tests/method-data-source.md).

```csharp
[Test]
[Arguments(3, 4, 5)]
[Arguments(0, 3, 4)]
public async Task NewXyz_Distance_MatchesLength(double x, double y, double z)
{
    // Arrange
    var expected = Math.Sqrt(x * x + y * y + z * z);

    // Act
    var point = Application.Create.NewXYZ(x, y, z);

    // Assert
    await Assert.That(point.DistanceTo(XYZ.Zero)).IsEqualTo(expected).Within(1e-6);
}
```

A data source runs during TUnit discovery, **off the Revit thread**; it yields plain inputs (numbers, strings, file paths) and never a Revit object.
For a seeded model, an opened sample file, an injected service, or the same test across many file kinds, use `revit-test-fixtures` — it routes each situation to the right fixture and data source.

### Step 4: Run against a matching Revit install

TUnit runs on Microsoft.Testing.Platform, and the configuration carries the target Revit version.

```shell
dotnet test -c Release.RNN
```

`RNN` is the target Revit-year configuration, for example `Release.R26`.
Use `dotnet run -c Release.RNN` for simpler command-line flag passing.
A licensed Revit matching the selected configuration must be installed, because the tests run against a real Revit process.

## Validation

- [ ] Tests inherit `RevitApiTest` and assert observable model behavior, not framework plumbing.
- [ ] Every Revit-touching hook carries `[HookExecutor<RevitThreadExecutor>]`.
- [ ] Data sources and inline arguments carry only primitives; Revit objects are built in the test body.
- [ ] Tests do not reference `RevitAPIUI` types that require a UI session.
- [ ] The selected `Release.RNN` configuration matches the installed Revit runtime.

## Common Pitfalls

| Pitfall                                                            | Correct approach                                                                                                    |
|--------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------|
| Revit API called with no executor (thread error)                   | Keep the assembly-level `TestExecutor<RevitThreadExecutor>`; add `[TestExecutor<...>]` only to override it.         |
| A Revit-touching `[Before]`/`[After]` hook without a hook executor | Add `[HookExecutor<RevitThreadExecutor>]`.                                                                          |
| A data source that returns a Revit object                          | Data sources run off the Revit thread at discovery; return primitives or paths and build Revit objects in the body. |
| A field initializer that loads a Revit API type                    | Field initializers run before Revit is injected; move the value into a lazy `field ??= …` property.                 |
| A test references a `RevitAPIUI` type that requires a UI session   | Reference only `RevitAPI` in tests; `RevitAPIUI` needs a UI session the test host lacks.                            |
| Asserting framework plumbing                                       | Assert the resulting model, file, or value.                                                                         |
| `RevitApiTest` not found                                           | The `Nice3point.TUnit.Revit` package is not referenced.                                                             |
