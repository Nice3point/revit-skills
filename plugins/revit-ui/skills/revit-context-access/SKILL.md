---
name: revit-context-access
description: >
  Access the ambient Autodesk Revit database application through RevitApiContext or an active UI session through Nice3point.Revit.Toolkit RevitContext, and suppress dialogs.
  USE FOR: reading RevitApiContext.Application in tests or Design Automation, reading the current UiApplication/ActiveDocument/ActiveView without a commandData, or suppressing TaskDialog and message boxes with BeginDialogSuppressionScope.
  DO NOT USE FOR: suppressing expected Revit warnings and failures during a transaction (use RevitApiContext.BeginFailureSuppressionScope).
license: MIT
---

# Revit Context Access

`RevitApiContext.Application` exposes the database-level Revit application for tests and Design Automation, where a UI session is unavailable.
`RevitContext` adds UI-session access to the active application, document, and view, plus bounded dialog suppression.

Use `RevitApiContext` for database-level access and failure suppression.
Use `RevitContext` only when a UI session is available.

## When to use

- Reading the database-level `Application` in a test or Design Automation.
- Reading the active document, UI application, or view from a service, helper, or window in a UI session.
- Suppressing predictable dialogs for a bounded operation.

## When not to use

- Inside an external command or application, where the base class already exposes the context — use `revit-command-and-application`.
- Accessing Revit from a test or Design Automation through UI types — use `RevitApiContext.Application`.
- Suppressing failures or warnings during a transaction — use `RevitApiContext.BeginFailureSuppressionScope`.

## Workflow

### Step 1: Read database-level context without a UI session

Use `RevitApiContext.Application` in tests and Design Automation.

```csharp
var application = RevitApiContext.Application;
```

### Step 2: Read UI-session context

```csharp
var uiApplication = RevitContext.UiApplication;
var document = RevitContext.ActiveDocument;
var view = RevitContext.ActiveView;
```

### Step 3: Suppress dialogs for a bounded operation

```csharp
using (RevitContext.BeginDialogSuppressionScope(TaskDialogResult.Ok))
{
    LoadFamilies();
}
```

Overloads accept a result code, a `TaskDialogResult`, a `MessageBoxResult`, or a custom handler that overrides the result.

### Step 4: Verify

Confirm database-only code accesses `RevitApiContext.Application`, UI-session code reads the expected active document, and a suppressed dialog no longer blocks the operation.

## Validation

- [ ] Tests and Design Automation access the database-level application through `RevitApiContext.Application`.
- [ ] UI-session access uses `RevitContext`, and nullable reads (`ActiveDocument`, `ActiveView`) are guarded.
- [ ] Dialog suppression is scoped to the operation that raises the dialog.

## Common Pitfalls

| Pitfall                                               | Correct approach                                                               |
|-------------------------------------------------------|--------------------------------------------------------------------------------|
| Using `RevitContext` in a test or Design Automation  | Use `RevitApiContext.Application`; it does not require a UI session.          |
| Assuming `RevitContext.ActiveDocument` is non-null    | It is nullable; guard when no document is open.                                |
| Suppressing a transaction failure with a dialog scope | Use `RevitApiContext.BeginFailureSuppressionScope`.                             |
| `RevitContext` not found                              | The `Nice3point.Revit.Toolkit` package is not referenced.                      |
