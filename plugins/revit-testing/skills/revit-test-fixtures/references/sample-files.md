# Sample Files

**Load when:** a test asserts against real file content from a fixed set of installed Revit sample files.

Enumerate the installed samples once as a **static** set, drive the tests with `[MethodDataSource]`, and open each file from a private copy so the original is never touched.
Requires the `Nice3point.TUnit.Revit` and `Nice3point.Revit.Injector` packages.

## Why a static set uses `[MethodDataSource]`

The path list is known at discovery and does not depend on any constructor state, so it lives in a **static** property and is referenced by `[MethodDataSource(nameof(DocumentPaths))]`.
When the set instead depends on constructor-injected configuration (extension, directory), it must be an instance member driven by `[InstanceMethodDataSource]` — see `parameterized-fixtures`.

The enumeration returns paths (primitives), so it is legal to run off the Revit thread at discovery; the document is opened inside the test on the Revit thread.

## Open a copy under failure suppression

A base class opens a private copy under `RevitApiContext.BeginFailureSuppressionScope()` so sample warnings do not block the open, and deletes the copy in teardown.
Clear the read-only attribute before deleting, because Revit may mark the copied file read-only.

```csharp
public abstract class StaticSampleDocumentFixture : RevitApiTest
{
    private string? _copyPath;

    public Document? Document { get; private set; }

    [After(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CloseDocument()
    {
        Document?.Close(false);
        if (_copyPath is null) return;

        File.SetAttributes(_copyPath, FileAttributes.Normal);
        File.Delete(_copyPath);
    }

    protected Document OpenDocument(string path)
    {
        _copyPath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}{Path.GetExtension(path)}");
        File.Copy(path, _copyPath);

        using (RevitApiContext.BeginFailureSuppressionScope())
        {
            Document = Application.OpenDocumentFile(_copyPath);
        }

        return Document;
    }
}
```

## Drive the tests

```csharp
public sealed class ModelSampleTests : StaticSampleDocumentFixture
{
    private static readonly string SamplesPath = $@"C:\Program Files\Autodesk\Revit {RevitEnvironment.MajorVersion}\Samples";

    public static string[] DocumentPaths { get; } = Directory.Exists(SamplesPath)
        ? Directory.EnumerateFiles(SamplesPath, "*.rvt").ToArray()
        : [];

    [Test]
    [MethodDataSource(nameof(DocumentPaths))]
    public async Task FilteredElementCollector_ElementTypes_AreAssignable(string path)
    {
        // Arrange
        var document = OpenDocument(path);

        // Act
        var elements = new FilteredElementCollector(document)
            .WhereElementIsElementType()
            .ToElements();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(elements).IsNotEmpty();
            await Assert.That(elements).All().Satisfy(element => element.IsAssignableTo<ElementType>());
        }
    }
}
```

## Notes

- An empty `DocumentPaths` produces no cases, so the tests are skipped rather than failing when Revit is not installed at the expected path — see `skipping`.
- Guard version-specific API behind `#if REVIT2025_OR_GREATER` inside the test body when a sample assertion differs across Revit versions.
- One document per test keeps sample tests isolated; never reuse a document opened by another test.
