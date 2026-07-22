---
name: revit-context-access
description: >
  Access the Autodesk Revit database application through RevitApiContext or an active UI session through RevitContext from Nice3point.Revit.Toolkit, and suppress dialogs.
  USE FOR: reading Application, the current UiApplication/ActiveDocument/ActiveView without a commandData, or suppressing TaskDialog and message boxes with BeginDialogSuppressionScope.
  DO NOT USE FOR: suppressing expected Revit warnings and failures during a transaction.
license: MIT
---

# Revit Context Access

## When to use

- Reading the database-level `Application`.
- Reading the active document, UI application, or view from a service, helper, or window in a UI session.
- Suppressing predictable dialogs for a bounded operation.

## When not to use

- Inside an external command or application, where the base class already exposes the context — use `revit-command-and-application`.
- Suppressing failures or warnings during a transaction — use `RevitApiContext.BeginFailureSuppressionScope`.

## Workflow

### Step 1: Read database-level context without a UI session

Use `RevitApiContext` for database-level access.

```csharp
var application = RevitApiContext.Application;
```

### Step 2: Read UI-session context

Use `RevitContext` for UIApplication, document, and view access only when a UI session is available.

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

Confirm database-only code accesses `RevitApiContext` without calling the `RevitContext`.

## Validation

- [ ] Tests and Design Automation access the database-level application through `RevitApiContext.Application`.
- [ ] UI-session access uses `RevitContext`, and nullable reads (`ActiveDocument`, `ActiveView`) are guarded.
- [ ] Dialog suppression is scoped to the operation that raises the dialog.

## Common Pitfalls

| Pitfall                                               | Correct approach                                          |
|-------------------------------------------------------|-----------------------------------------------------------|
| Using `RevitContext` in a test or Design Automation   | Use `RevitApiContext`; it does not require a UI session.  |
| Assuming `RevitContext.ActiveDocument` is non-null    | It is nullable; guard when no document is open.           |
| Suppressing a transaction failure with a dialog scope | Use `RevitApiContext.BeginFailureSuppressionScope`.       |
| `RevitContext` not found                              | The `Nice3point.Revit.Toolkit` package is not referenced. |
