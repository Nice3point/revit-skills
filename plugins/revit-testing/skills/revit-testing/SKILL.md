---
name: revit-testing
description: >
    Write, run, or review Autodesk Revit API tests that execute inside Revit with Nice3point.TUnit.Revit.
    USE FOR: writing RevitApiTest classes whose bodies run on Revit's single thread, and RevitApiUiTest classes for code that needs the Revit user interface, the UIApplication, or the active UI document.
    DO NOT USE FOR: supplying the documents, services, or data cases a test runs against (use revit-test-fixtures), scaffolding the test project (create it from the revit-tunit template), or tests that never call the Revit API.
license: MIT
---

# Revit Testing

Every Revit API call must run on the single thread that initialized Revit.
`Nice3point.TUnit.Revit` runs each test and hook on that thread, and the test body calls the Revit API without dispatch code.
The package is based on TUnit and Microsoft.Testing.Platform.
A test run requires a matching licensed Revit installation.

A project scaffolded from the `revit-tunit` template already contains the project structure.

## When to use

- Writing or reviewing a test for project logic or helpers that call the Revit API.
- Asserting that an operation produced the expected model, file, or value.
- Testing code that reads the `UIApplication`, the active `UIDocument`, the selection, the open views, the ribbon, or the postable commands.

## When not to use

- Providing the document, service, or parameterized cases a test consumes — use `revit-test-fixtures`.
- Scaffolding the test project itself — create it from the `revit-tunit` template.
- The code under test never calls the Revit API — write a plain TUnit test with no Revit base class and no executor.

## Workflow

### Step 1: Choose the base class

| Code under test                                                                      | Base class       | Exposes         |
|--------------------------------------------------------------------------------------|------------------|-----------------|
| Calls `RevitAPI` only: documents, elements, geometry, transactions, and so on        | `RevitApiTest`   | `Application`   |
| Needs the Revit user interface: `UIApplication`, `UIDocument`, the ribbon, and so on | `RevitApiUiTest` | `UiApplication` |

`RevitApiTest` is the recommended base class: it runs without the Revit user interface and has better performance and stability.
`RevitApiUiTest` starts Revit with its user interface and applies only to code that requires it.

**Load when:** the test derives from `RevitApiUiTest` or references a `RevitAPIUI` type — read [references/user-interface-tests.md](references/user-interface-tests.md).

### Step 2: Write a test on the Revit thread

The base class exposes the shared `Application` or `UiApplication`.
TUnit constructs a new instance of the test class for every test.
Instance fields and properties are isolated between tests and store per-test state.
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

`Assert.Multiple()` groups related checks and reports every failure in the group.
TUnit assertions are awaited: `IsEqualTo(...).Within(tol)` for doubles, `IsTrue()`/`IsNotEmpty()` for booleans and emptiness checks, `.All().Satisfy(...)` for collections, and `.Throws<TException>()` for failures.

A UI test reads the user interface through `UiApplication` in the same shape:

```csharp
public sealed class SelectionTests : RevitApiUiTest
{
    [Test]
    public async Task SetElementIds_ActiveDocument_SelectsTheLevels()
    {
        // Arrange
        var uiDocument = UiApplication.OpenAndActivateDocument(modelPath);
        var levelIds = uiDocument.Document.CollectElements()
            .OfClass<Level>()
            .ToElementIds();

        // Act
        uiDocument.Selection.SetElementIds(levelIds);

        // Assert
        await Assert.That(uiDocument.Selection.GetElementIds()).IsEquivalentTo(levelIds);
    }
}
```

### Step 3: Run every Revit API member on the Revit thread

`RevitApiTest` and `RevitApiUiTest` apply their executor to every test and hook of the class; a test or a `[Before]`/`[After]` hook needs no executor attribute.

- A test that must run off the Revit thread overrides with its own `[TestExecutor<OtherExecutor>]`.
- Load Revit API types lazily — a field initializer runs at construction, before Revit is injected:

```csharp
// BAD — runs before Revit initialized.
private readonly ElementId _levelId = new ElementId(BuiltInCategory.OST_Levels);

// GOOD — resolved on first use, on the Revit thread.
private ElementId LevelId => field ??= new ElementId(BuiltInCategory.OST_Levels);
```

Calling the Revit API during test discovery causes an InvalidOperationException: "Attempted to write protected memory."
Discovery happens before Revit is injected and off its thread: TUnit constructs the test class, evaluates every data source, and resolves every dependency-injection service at discovery.
No constructor, field initializer, data-source member, or injected service may call the Revit API at construction.
Move that work into the test body or a `[Before]` hook.
`Application` and `UiApplication` throw an `InvalidOperationException` outside a test body or a hook.

### Step 4: Parameterize and supply fixtures

Pass a small fixed set of primitive cases inline with `[Arguments]`, and build the Revit objects in the test body.
For a seeded model, an opened sample file, an injected service, or the same test across many file kinds, use `revit-test-fixtures`; it maps each situation to a fixture and a data source.
A test fixture is a basic TUnit feature, and the Revit thread is the only constraint Revit adds to it.

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

A data source runs during TUnit discovery, **off the Revit thread**.
It returns plain inputs (numbers, strings, file paths) and never a Revit object.

### Step 5: Run against a matching Revit install

TUnit runs on Microsoft.Testing.Platform, and the build configuration selects the target Revit version.

```shell
dotnet test -c Release.RNN
```

`RNN` is the target Revit-year configuration, for example `Release.R26`.
Use `dotnet run -c Release.RNN` for simpler command-line flag passing.
A licensed Revit matching the selected configuration must be installed; the tests run against a real Revit process.
A run with UI tests starts a separate Revit process with its user interface.

## Validation

- [ ] Tests inherit `RevitApiTest`, or `RevitApiUiTest` where the code under test needs the Revit user interface, and assert observable model behavior, not framework plumbing.
- [ ] Data sources and inline arguments pass only primitives; Revit objects are built in the test body.
- [ ] Only `RevitApiUiTest` classes reference `RevitAPIUI` types, and the test project references `Nice3point.Revit.Api.RevitAPIUI`.
- [ ] A UI test removes the ribbon panels and other user interface changes it adds.
- [ ] The selected `Release.RNN` configuration matches the installed Revit runtime.

## Common Pitfalls

| Pitfall                                                          | Correct approach                                                                                                    |
|------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------|
| Revit API called with no executor (thread error)                 | Derive the class from `RevitApiTest` or `RevitApiUiTest`; add `[TestExecutor<...>]` only to override it.            |
| A data source that returns a Revit object                        | Data sources run off the Revit thread at discovery; return primitives or paths and build Revit objects in the body. |
| A field initializer that loads a Revit API type                  | Field initializers run before Revit is injected; move the value into a lazy `field ??= …` property.                 |
| A `RevitApiTest` references a `RevitAPIUI` type                  | Derive the class from `RevitApiUiTest`; `RevitApiTest` runs without the Revit user interface.                       |
| `UiApplication` read in a constructor, a field, or a data source | Read it in the test body or a hook; outside them it throws `InvalidOperationException`.                             |
| A UI test does not remove a ribbon panel or another UI change    | Remove it in a `finally` block or an `[After(Test)]` hook; every UI test of the session shares one Revit interface. |
| A timed-out UI test blocks the next one                          | Accept the `CancellationToken` of the test and pass it to long-running work.                                        |
| Asserting framework plumbing                                     | Assert the resulting model, file, or value.                                                                         |
| `RevitApiTest` or `RevitApiUiTest` not found                     | The `Nice3point.TUnit.Revit` package is not referenced.                                                             |
