---
name: revit-context-menu
description: >
  Add Autodesk Revit right-click context menu entries with the Nice3point.Revit.Extensions fluent API.
  USE FOR: registering context menu items, submenus, and separators bound to command classes during application startup.
  DO NOT USE FOR: ribbon panels and buttons (use revit-ribbon).
license: MIT
---

# Revit Context Menu

`ConfigureContextMenu` (from `Nice3point.Revit.Extensions`) wraps the raw context-menu registration into a fluent call.
Register the menu once during application startup.

## When to use

- Adding entries to Revit's right-click context menu, bound to command classes.

## When not to use

- Building ribbon panels and buttons — use `revit-ribbon`.

## Workflow

### Step 1: Configure the menu

`AddMenuItem<TCommand>` binds a menu item to a command type; `AddSeparator` inserts a divider; `AddSubMenu` nests a `ContextMenu`.

```csharp
application.ConfigureContextMenu(menu =>
{
    menu.AddMenuItem<AnalyzeCommand>("Analyze selection");
    menu.AddSeparator();

    var exportMenu = new ContextMenu();
    exportMenu.AddMenuItem<ExportCommand>("Export selection");
    menu.AddSubMenu("Export", exportMenu);
});
```

Use the `ConfigureContextMenu(title, configuration)` overload to title the menu group.

### Step 2: Verify

Right-click in Revit and confirm the items and submenu appear and each invokes its command.

## Validation

- [ ] The menu is configured once during application startup.
- [ ] Items bind to command types via `AddMenuItem<TCommand>`.
- [ ] Submenus are built as a `ContextMenu` and added with `AddSubMenu`.

## Common Pitfalls

| Pitfall                                            | Correct approach                                                  |
|----------------------------------------------------|-------------------------------------------------------------------|
| Registering context menu items through the raw API | Use `ConfigureContextMenu` with `AddMenuItem<TCommand>`.          |
| Expecting `AddSubMenu` to take a lambda            | Build a `ContextMenu` and pass it to `AddSubMenu(name, subMenu)`. |
| `ConfigureContextMenu` not found                   | The `Nice3point.Revit.Extensions` package is not referenced.      |
