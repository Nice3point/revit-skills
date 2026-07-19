# Seeded Document

**Load when:** a test needs a fresh, controlled model whose exact contents you author in code.

Build the model in memory per test so every test starts from a known state and nothing leaks between tests.
Requires the `Nice3point.TUnit.Revit` package.

## Fresh document per test

Create the document in `[Before(Test)]`, seed it inside a transaction, and close it in `[After(Test)]`.
Both hooks run on the Revit thread via `[HookExecutor<RevitThreadExecutor>]`.
Store the seeded elements you assert against so the test reads intent, not rediscovery.

```csharp
public sealed class WallModelTests : RevitApiTest
{
    private Document _document = null!;
    private IList<Wall> _exteriorWalls = null!;

    [Before(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void SeedModel()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using var transaction = new Transaction(_document, "Seed model");
        transaction.Start();

        var level = Level.Create(_document, 0);
        _exteriorWalls =
        [
            Wall.Create(_document, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)), level.Id, false),
            Wall.Create(_document, Line.CreateBound(new XYZ(10, 0, 0), new XYZ(10, 6, 0)), level.Id, false),
            Wall.Create(_document, Line.CreateBound(new XYZ(10, 6, 0), new XYZ(0, 6, 0)), level.Id, false),
            Wall.Create(_document, Line.CreateBound(new XYZ(0, 6, 0), new XYZ(0, 0, 0)), level.Id, false),
        ];

        transaction.Commit();
    }

    [After(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CloseModel()
    {
        _document.Close(false);
    }

    [Test]
    public async Task FilteredElementCollector_ExteriorWalls_MatchSeededCount()
    {
        // Arrange
        var expectedCount = _exteriorWalls.Count;

        // Act
        var walls = new FilteredElementCollector(_document)
            .WhereElementIsNotElementType()
            .OfClass(typeof(Wall))
            .ToList();

        // Assert
        await Assert.That(walls.Count).IsEqualTo(expectedCount);
    }

    [Test]
    public async Task Transaction_DemolishWall_RemainingWallCountDecreases()
    {
        // Arrange
        var targetId = _exteriorWalls[0].Id;

        // Act
        using var transaction = new Transaction(_document, "Demolish wall");
        transaction.Start();
        _document.Delete(targetId);
        transaction.Commit();

        var remaining = new FilteredElementCollector(_document)
            .WhereElementIsNotElementType()
            .OfClass(typeof(Wall))
            .ToElementIds();

        // Assert
        await Assert.That(remaining.Count).IsEqualTo(_exteriorWalls.Count - 1);
    }
}
```

## Read-only tests share one document

When every test only reads the model, seed it once in `[Before(Class)]` and close it in `[After(Class)]`.
This is faster, and safe precisely because no test mutates shared state.
Switch back to `[Before(Test)]` the moment a test writes.

## Notes

- Each test runs on a fresh instance of the class, so the `_document` and seeded-element fields never leak between tests.
- `Application.NewProjectDocument(UnitSystem.Metric)` returns an unsaved in-memory document; `Close(false)` discards it without a save prompt.
- Field initializers run at construction, before Revit is injected, so build seeded state inside the hook, not in a field initializer.
- Parameterize the seed with inline `[Arguments]` (primitives only) when a few variants of the same model are needed.
