---
name: revit-dockable-pane
description: >
  Register a WPF dockable pane in the Autodesk Revit UI with Nice3point.Revit.Toolkit DockablePaneProvider.
  USE FOR: registering a dockable pane and its initial dock state during application startup with the fluent Register().SetConfiguration() API.
license: MIT
---

# Revit Dockable Pane

`DockablePaneProvider.Register(...).SetConfiguration(...)` (from `Nice3point.Revit.Toolkit`) wraps the verbose `IDockablePaneProvider` registration into a fluent call.
Register the pane once during application startup.

## When to use

- Adding a WPF dockable pane to the Revit UI and setting its initial dock position.

## Workflow

### Step 1: Register during startup

```csharp
DockablePaneProvider
    .Register(application, paneId, "Analysis")
    .SetConfiguration(data =>
    {
        data.FrameworkElement = new AnalysisPaneView();
        data.InitialState = new DockablePaneState
        {
            MinimumWidth = 300,
            DockPosition = DockPosition.Right
        };
    });
```

### Step 2: Resolve the view from DI when needed

To build the pane's element from a container instead of `new`-ing it, set `data.FrameworkElementCreator` to a `FrameworkElementCreator<T>` backed by the service provider (from `Nice3point.Revit.Toolkit`):

```csharp
data.FrameworkElementCreator = new FrameworkElementCreator<AnalysisPaneView>(serviceProvider);
```

### Step 3: Verify

Confirm the pane appears under the Revit view tab and docks in the configured position.

## Validation

- [ ] The pane is registered once during application startup with a stable pane id.
- [ ] `FrameworkElement` (or `FrameworkElementCreator`) and `InitialState` are set.

## Common Pitfalls

| Pitfall                                      | Correct approach                                                |
|----------------------------------------------|-----------------------------------------------------------------|
| Implementing `IDockablePaneProvider` by hand | Use `DockablePaneProvider.Register(...).SetConfiguration(...)`. |
| A new pane id (`Guid`) generated per run     | Use a stable, persisted pane id.                                |
| `DockablePaneProvider` not found             | The `Nice3point.Revit.Toolkit` package is not referenced.       |
