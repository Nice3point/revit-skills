---
name: revit-command-and-application
description: >
  Build Autodesk Revit add-in entry points with Nice3point.Revit.Toolkit base classes instead of raw IExternalCommand/IExternalApplication interfaces.
  USE FOR: authoring an external command or application with a simplified Execute()/OnStartup() override, optional OnShutdown() override, ready context properties, and automatic dependency resolution.
  DO NOT USE FOR: dispatching API work from code that runs outside the Revit API context (use revit-external-events).
license: MIT
---

# Revit Command and Application

Derive add-in entry points from the `Nice3point.Revit.Toolkit` base classes.
They replace the raw interface boilerplate (the full `Execute(commandData, ref message, elements)` signature, manual assembly resolution) with a simplified override plus ready-made context properties.

## When to use

- Authoring an `IExternalCommand`, `IExternalApplication`, or `IExternalDBApplication` entry point.
- Reviewing an entry point that reimplements the raw interface by hand.

## When not to use

- Building the ribbon inside `OnStartup` — that is `revit-ribbon`.
- Invoking the API from a modeless window or background thread — that is `revit-external-events`.

## Workflow

### Step 1: Derive an external command

Override `Execute()`; reach the context through the inherited `Application` instead of unpacking `commandData`.

```csharp
[Transaction(TransactionMode.Manual)]
public class DeleteWallsCommand : ExternalCommand
{
    public override void Execute()
    {
        var document = Application.ActiveUIDocument.Document;
    }
}
```

`ExternalCommand` exposes high-level `Application`, `View`, `JournalData`, and `ElementSet` properties.
`Result` defaults to `Succeeded`; set `Result` and `ErrorMessage` only when the command must return a canceled or failed outcome.

### Step 2: Derive an external application

```csharp
public class AddinApplication : ExternalApplication
{
    public override void OnStartup()
    {
        var panel = Application.CreatePanel("Commands", "RevitAddin");
        panel.AddPushButton<DeleteWallsCommand>("Delete walls");
    }
}
```

Use `ExternalDBApplication` for a database-only application.
Override `OnShutdown()` only when the application has session cleanup to perform; it is optional.
The ribbon helpers (`CreatePanel`, `AddPushButton<T>`) come from `Nice3point.Revit.Extensions` — see `revit-ribbon`.

### Step 3: Use the async base classes for long-running work

Derive from `AsyncExternalCommand` and override `ExecuteAsync()`, or from `AsyncExternalApplication` and override `OnStartupAsync()`/`OnShutdownAsync()`.
The sealed base pumps the message loop on Revit's main thread; `await` keeps the UI responsive while the continuation still runs in the API context — no external event needed.

```csharp
[Transaction(TransactionMode.Manual)]
public class ExportViewsCommand : AsyncExternalCommand
{
    public override async Task ExecuteAsync()
    {
        var settings = await LoadSettingsAsync(); // long-running work; the UI stays responsive
        CreateRibbon(settings);
    }
}
```

### Step 4: Rely on automatic dependency resolution

The base classes open an assembly-resolve scope automatically; add-in dependencies load from the add-in folder without a manual `AssemblyResolve` handler.
For dependencies loaded outside an entry point, open a scope explicitly with `ResolveHelper.BeginAssemblyResolveScope`.

### Step 5: Verify

Test the observable model behavior on the Revit thread.
Leave the default successful result unless the command must cancel or fail.

## Validation

- [ ] The entry point derives from a Toolkit base class, not a hand-written interface implementation.
- [ ] Context comes from the inherited properties, not manual `commandData` unpacking.
- [ ] An application overrides `OnShutdown()` only when it has session cleanup to perform.
- [ ] Long-running UI work uses `AsyncExternalCommand`.
- [ ] The command has the expected observable behavior in Revit.
- [ ] `Result` and `ErrorMessage` are set only for a canceled or failed command.

## Common Pitfalls

| Pitfall                                                            | Correct approach                                                                               |
|--------------------------------------------------------------------|------------------------------------------------------------------------------------------------|
| Implementing `Execute(commandData, ref message, elements)` by hand | Derive from `ExternalCommand` and override `Execute()`.                                        |
| Manual `AppDomain.AssemblyResolve` for add-in dependencies         | The base classes resolve automatically; else open a `ResolveHelper.BeginAssemblyResolveScope`. |
| Blocking the UI in a long command                                  | Use `AsyncExternalCommand` and `ExecuteAsync()`.                                               |
| `ExternalCommand` not found                                        | The `Nice3point.Revit.Toolkit` package is not referenced.                                      |
