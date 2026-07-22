# Dependency-Injection Data Source

**Load when:** the code under test is a service that should run with its real dependencies (logging, options, collaborators) built by a DI container.

Feed the test class's constructor from an `IServiceProvider` by subclassing TUnit's `DependencyInjectionDataSourceAttribute<TScope>`, then apply that attribute to the test class.
This exercises the real object graph instead of hand-assembling collaborators in every test.
Requires the `Nice3point.TUnit.Revit`, TUnit, and `Microsoft.Extensions.DependencyInjection` packages.

## Author the attribute once

Build the provider a single time and hand out a scope per test.
Register exactly the services the tests need, with real or test-friendly implementations.

```csharp
public sealed class ServiceInjectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider Provider = BuildProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata metadata)
    {
        return Provider.CreateScope();
    }

    public override object? Create(IServiceScope scope, Type type)
    {
        return scope.ServiceProvider.GetService(type);
    }

    private static IServiceProvider BuildProvider()
    {
        return new ServiceCollection()
            .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning))
            .AddTransient<ElementInspector>()
            .BuildServiceProvider();
    }
}
```

## Inject into the test class

Apply the attribute to the class and take the services in its primary constructor.
The class can still inherit `RevitApiTest`, seed a document on the Revit thread, and run its assertions against the injected service.

```csharp
[ServiceInjectionDataSource]
public sealed class ElementInspectorTests(ElementInspector elementInspector) : RevitApiTest
{
    private Document _document = null!;
    private Wall _wall = null!;

    [Before(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void SeedModel()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using var transaction = new Transaction(_document, "Seed model");
        transaction.Start();
        
        var level = Level.Create(_document, 0);
        _wall = Wall.Create(_document, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)), level.Id, false);
        
        transaction.Commit();
    }

    [After(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CloseModel()
    {
        _document.Close(false);
    }

    [Test]
    public async Task Inspect_NullElement_ReturnsNull()
    {
        // Arrange
        Element? element = null;

        // Act
        var result = elementInspector.Inspect(element);

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Inspect_Wall_ReturnsCategoryName()
    {
        // Arrange
        var wall = _wall;

        // Act
        var result = elementInspector.Inspect(wall);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.CategoryName).IsNotEmpty();
    }
}
```

## Compose with a per-test data source

Dependency injection fills the **constructor**; a method data source still fills each **test parameter**.
Put the DI attribute on the class and `[MethodDataSource]` / `[InstanceMethodDataSource]` (see `parameterized-fixtures`) on the method — for example, a service test that runs over sample paths:

```csharp
[ServiceInjectionDataSource]
public sealed class ElementInspectorOverSamplesTests(ElementInspector elementInspector) : DocumentSampleFixture(".rvt")
{
    [Test]
    [InstanceMethodDataSource(nameof(DocumentPaths))]
    public async Task Inspect_SampleElement_ReturnsValue(string path)
    {
        // Arrange
        var document = OpenDocument(path);
        var element = new FilteredElementCollector(document).WhereElementIsNotElementType().First();

        // Act
        var result = elementInspector.Inspect(element);

        // Assert
        await Assert.That(result).IsNotNull();
    }
}
```

## Notes

- The provider is built and services are resolved during discovery, before Revit is injected, so a service's constructor and field initializers must not call the Revit API; defer Revit calls to methods the test body invokes on the Revit thread.
- Keep the provider `static`: one container is built for the whole run, and each test gets its own scope for scoped/transient services.
- Register test doubles here (in-memory options, a fake storage service) when a real dependency would touch the network or disk.
- For data-source patterns beyond DI, use the TUnit documentation linked from `revit-test-fixtures`; return only plain inputs that the Revit-thread test body can consume.
