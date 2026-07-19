---
name: revit-failure-handling
description: >
  Suppress expected Autodesk Revit warnings and failures during a bounded API operation with Nice3point.Revit.Toolkit RevitApiContext.
  USE FOR: auto-resolving or auto-cancelling a known failure that would otherwise block opening a document, loading a family, or committing a transaction.
  DO NOT USE FOR: suppressing UI dialogs that are not failures (use RevitContext.BeginDialogSuppressionScope), or running API work from outside the Revit API context (use a Toolkit external event).
license: MIT
---

# Revit Failure Handling

`RevitApiContext.BeginFailureSuppressionScope` (from `Nice3point.Revit.Toolkit`) returns a disposable scope that resolves or cancels Revit failures without showing a user dialog.
Use it only for a known failure the operation can safely handle — never to make an unknown failure appear successful.

## When to use

- A predictable warning or recoverable error blocks a headless operation (opening, family load, transaction commit).
- Reviewing that suppression is bounded to the operation that raises the failure.

## When not to use

- The interruption is a UI dialog rather than a failure — use `RevitContext.BeginDialogSuppressionScope`.
- The API is being called from outside the API context — dispatch the work through a Toolkit external event.

## Workflow

### Step 1: Confirm the failure is expected

Reproduce it with the smallest representative model, confirm it is a warning or recoverable error (not a real processing failure), and confirm that resolving or cancelling it preserves the required output.
Keep the failure details in structured logs or test diagnostics.

### Step 2: Bound the suppression scope

Wrap only the operation that owns the expected failure; leave unrelated work outside the scope.
The scope hooks the application-level `FailuresProcessing` event; it covers both a transaction commit and a document open.

```csharp
using (RevitApiContext.BeginFailureSuppressionScope())
{
    using var transaction = new Transaction(document, "Operation");
    transaction.Start();
    // ... operation that raises the expected failure
    transaction.Commit();
}
```

Opening a file can raise the same recoverable warnings.
In an unattended or headless host (for example Autodesk Design Automation) no user can dismiss the dialog; open the document inside the scope:

```csharp
using (RevitApiContext.BeginFailureSuppressionScope())
{
    var document = RevitApiContext.Application.OpenDocumentFile(path);
}
```

### Step 3: Choose resolve or cancel

The default resolves resolvable failures.
Pass `resolveErrors: false` when the caller must instead observe unresolved errors rather than have them auto-resolved.

### Step 4: Verify

Assert the resulting document or artifact is valid, and confirm an unrelated failure still reaches the normal error boundary.

## Validation

- [ ] The suppressed failure is known and expected.
- [ ] The scope covers one bounded operation, not unrelated work.
- [ ] The resulting model or file remains valid.
- [ ] Unrelated failures remain observable.

## Common Pitfalls

| Pitfall                                       | Correct approach                                                      |
|-----------------------------------------------|-----------------------------------------------------------------------|
| Wrapping a whole pipeline to hide any failure | Scope suppression to the one operation that raises the known failure. |
| Suppressing a UI dialog with a failure scope  | Use `RevitContext.BeginDialogSuppressionScope`.                       |
| Auto-resolving errors the caller must see     | Pass `resolveErrors: false`.                                          |
| `RevitApiContext` not found                   | The `Nice3point.Revit.Toolkit` package is not referenced.             |
