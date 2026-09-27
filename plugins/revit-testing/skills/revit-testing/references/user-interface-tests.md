# User interface tests

`RevitApiUiTest` runs a test inside a Revit process started with its user interface.
The static `UiApplication` property returns the `UIApplication` of that process, and `UiApplication.Application` returns its `Application`.
The base class applies its executor to every test body and hook of the class, and each of them runs on the Revit thread inside a Revit API context.

## Reference the user interface assembly

`Nice3point.TUnit.Revit` does not pass `RevitAPIUI` on to the test project.
The test project references it next to `RevitAPI`:

```xml
<PackageReference Include="Nice3point.Revit.Api.RevitAPIUI" Version="$(RevitVersion).*"/>
```

## Write the test

The test body and its hooks use `UIApplication`, `UIDocument`, the selection, the open views, the ribbon, and the postable commands as an add-in does.

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

    [Test]
    public async Task CanPostCommand_BuiltInCommand_IsPostable()
    {
        // Arrange
        var commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.Default3DView);

        // Act
        var canPost = UiApplication.CanPostCommand(commandId);

        // Assert
        await Assert.That(canPost).IsTrue();
    }
}
```

Every UI test of a test session shares one Revit user interface.
A test that adds a ribbon panel, a button, or another change to the user interface removes it in a `finally` block or an `[After(Test)]` hook:

```csharp
var panel = UiApplication.AsControlledApplication().CreatePanel("Tests", "TUnit");
try
{
    var button = panel.AddPushButton<EmptyCommand>("Run");

    await Assert.That(button.ClassName).IsEqualTo(typeof(EmptyCommand).FullName);
}
finally
{
    panel.RemovePanel();
}
```

`UiApplication` throws an `InvalidOperationException` outside a test body or a hook.
A constructor, a field initializer, and a data source never read it.

## Lifetime and scheduling

- Revit starts with its user interface on the first UI test a test session executes, and closes when the test session finishes.
- The duration of a UI test covers its hooks and body inside Revit, and excludes the start of Revit.
- UI tests run one at a time, in parallel with `RevitApiTest` tests.
- The output a UI test writes to the console appears in the output of that test in the test host.

A UI test class that must run sequentially with the `RevitApiTest` tests is marked with `[ParallelLimiter<RevitParallelLimit>]`:

```csharp
[ParallelLimiter<RevitParallelLimit>]
public sealed class SelectionTests : RevitApiUiTest
{
}
```

A UI test with a `[Timeout]` accepts a `CancellationToken` and passes it to long-running work.
The next UI test starts only once the body of the timed-out test returns.

```csharp
[Test]
[Timeout(30_000)]
public async Task ExportAsync_ActiveDocument_WritesTheFile(CancellationToken cancellationToken)
{
    // Arrange
    var uiDocument = UiApplication.OpenAndActivateDocument(modelPath);

    // Act
    var path = await _exporter.ExportAsync(uiDocument.Document, cancellationToken);

    // Assert
    await Assert.That(File.Exists(path)).IsTrue();
}
```
