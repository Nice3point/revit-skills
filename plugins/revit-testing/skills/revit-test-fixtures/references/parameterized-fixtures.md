# Parameterized Fixtures

**Load when:** the same test body must run across several file kinds or sample directories (families, projects, or a project folder) without duplicating the tests.

Write the tests once in an abstract base parameterized by constructor arguments, then declare one sealed subclass per configuration.
Because the path set now depends on constructor state, it is an **instance** member driven by `[InstanceMethodDataSource]`, and each subclass re-runs the inherited tests with `[InheritsTests]`.
Requires the `Nice3point.TUnit.Revit` and `Nice3point.Revit.Injector` packages.

## `[InstanceMethodDataSource]` versus `[MethodDataSource]`

`[MethodDataSource]` reads a **static** member, evaluated before any instance exists, so it cannot see per-subclass configuration.
`[InstanceMethodDataSource]` constructs the instance first, then reads the member — so a `DocumentPaths` computed from the constructor's `extension` and `directory` resolves to the right set for each subclass.
Use the instance variant whenever the case set depends on constructor-injected state; use the static variant only for a truly fixed set (see `sample-files`).

## Base class parameterized by constructor

```csharp
public abstract class DocumentSampleFixture : RevitApiTest
{
    private string? _copyPath;

    protected DocumentSampleFixture(string extension, string? samplesDirectory = null)
    {
        var directory = samplesDirectory ?? $@"C:\Program Files\Autodesk\Revit {RevitEnvironment.MajorVersion}\Samples";
        DocumentPaths = Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, $"*{extension}").ToArray()
            : [];
    }

    // Instance member: value depends on the constructor, so tests use [InstanceMethodDataSource].
    public string[] DocumentPaths { get; }
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

## Put the shared test below the fixture

The fixture opens and closes documents only.
Put `[Test]` and `[InstanceMethodDataSource]` in a separate abstract test class so the fixture remains reusable for tests with different assertions.

```csharp
public abstract class DocumentSampleTests(string extension, string? samplesDirectory = null) : DocumentSampleFixture(extension, samplesDirectory)
{
    [Test]
    [InstanceMethodDataSource(nameof(DocumentPaths))]
    public async Task Open_ValidSample_HasElements(string path)
    {
        // Arrange
        var document = OpenDocument(path);

        // Act
        var elements = new FilteredElementCollector(document)
            .WhereElementIsNotElementType()
            .ToElements();

        // Assert
        await Assert.That(elements).IsNotEmpty();
    }
}
```

## One subclass per configuration

Each subclass fixes its extension and directory and carries `[InheritsTests]` so the base tests run again under its configuration.
A subclass may add tests that only make sense for its kind.

```csharp
[InheritsTests]
public sealed class FamilySampleTests() : DocumentSampleTests(".rfa")
{
    [Test]
    [InstanceMethodDataSource(nameof(DocumentPaths))]
    public async Task Open_FamilyFile_IsFamilyDocument(string path)
    {
        // Arrange
        var document = OpenDocument(path);

        // Act
        var isFamilyDocument = document.IsFamilyDocument;

        // Assert
        await Assert.That(isFamilyDocument).IsTrue();
    }
}

[InheritsTests]
public sealed class ProjectSampleTests() : DocumentSampleTests(".rvt");

[InheritsTests]
public sealed class ProjectFolderSampleTests() : DocumentSampleTests(".rvt", "./samples/projects");
```

## Notes

- Combine this with a dependency-injection data source to have the constructor also receive services under test — see `dependency-injection`; the DI attribute goes on each concrete subclass, and `[InstanceMethodDataSource]` still fills the per-test parameter.
- A subclass whose directory is empty produces no cases and is skipped — see `skipping`.
- Keep the base class abstract; only sealed subclasses are discovered as test classes.
