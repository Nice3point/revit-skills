---
name: revit-external-events
description: >
  Run Autodesk Revit API work from code that executes outside the Revit API context, using Nice3point.Revit.Toolkit external events.
  USE FOR: raising API work from a modeless window, background thread, or other non-API-context callback via the toolkit's external events or the [ExternalEvent] source generator.
  DO NOT USE FOR: code that already runs in the Revit API context — call the API directly.
license: MIT
---

# Revit External Events

The Revit API may only be touched inside its API context.
`Nice3point.Revit.Toolkit` external events auto-initialize, so you can construct and `Raise` them from any thread without creating them inside the API context first.

## When to use

- Invoking the API from a modeless WPF window, a background task, or another non-API-context callback.
- Reviewing that code outside the API context does not touch the API directly.

## When not to use

- The caller already runs in the API context (an external command, a Revit event handler, a Revit-invoked callback) — call the API directly.
- The window is shown modally with `ShowDialog`: it runs on the API thread, where the API is reachable directly.

## Workflow

### Step 1: Choose the event shape

- `ExternalEvent` / `ExternalEvent<T>` — fire-and-forget with zero or one argument.
- `AsyncExternalEvent` / `AsyncExternalEvent<T>` — await completion.
- `AsyncRequestExternalEvent<TResult>` / `<T, TResult>` — await a returned result.

### Step 2: Prefer the [ExternalEvent] generator

Mark every containing type `partial` and annotate a handler method with `[ExternalEvent]`.
The generator emits the event member from the method signature and names it after the method — `<Method>Event` for the synchronous form and `<Method>AsyncEvent` for the awaitable one — and you raise the work through that generated property.
Keep the transaction inside the method.

```csharp
public sealed partial class WindowCleaner
{
    [ExternalEvent]
    private void DeleteWindows(UIApplication application)
    {
        var document = application.ActiveUIDocument.Document;
        using var transaction = new Transaction(document, "Delete windows");
        transaction.Start();
        document.Delete(document.GetInstanceIds(BuiltInCategory.OST_Windows));
        transaction.Commit();
    }

    // from any thread or context
    private void OnDeleteClick() => DeleteWindowsEvent.Raise();
}
```

A value-returning method generates only the awaitable request form (`<Method>AsyncEvent`, raised with `await`); a method with two or more extra parameters also gets a generated `Raise(...)`/`RaiseAsync(...)` taking the individual arguments.

### Step 3: Construct an event manually when the generator does not fit

Use the public constructors for a lambda or field, a non-`partial` type, a DI-created instance, or a signature the generator rejects.
Each event type offers two handler families — one that takes the argument(s) only, and one that also receives the current `UIApplication` — and every family has an optional `ExternalEventOptions` overload.
(There is no `ExternalEvent.Create(...)` factory; the constructor is the manual entry point.)

```csharp
private readonly ExternalEvent<ElementId> _deleteElement = new((application, elementId) =>
{
    var document = application.ActiveUIDocument.Document;
    using var transaction = new Transaction(document, "Delete element");
    transaction.Start();
    document.Delete(elementId);
    transaction.Commit();
});

// from any thread or context
_deleteElement.Raise(elementId);
```

Omit `UIApplication` when you do not need it (`new ExternalEvent(() => …)`, `new ExternalEvent<T>(arg => …)`) and read ambient state through `RevitContext` instead.
Await completion with `AsyncExternalEvent`/`AsyncExternalEvent<T>` (`await …RaiseAsync()`), and return a value with `AsyncRequestExternalEvent<TResult>`/`<T, TResult>` (`var result = await …RaiseAsync(arg)`).

### Step 4: Allow direct invocation only for dual-context operations

Set `[ExternalEvent(AllowDirectInvocation = true)]` only when one operation must serve callers inside and outside the API context; it runs inline when already in API mode.
Never use it to bypass ownership or concurrency rules.

### Step 5: Verify

Await async event results before consuming generated Revit objects, and test the observable model result.

## Validation

- [ ] Code outside the API context reaches the API through an external event.
- [ ] Transactions stay inside the event callback or annotated method.
- [ ] An `[ExternalEvent]` method's containing type is `partial`.
- [ ] `AllowDirectInvocation` is set only for a real dual-context need.

## Common Pitfalls

| Pitfall                                                                     | Correct approach                                                |
|-----------------------------------------------------------------------------|-----------------------------------------------------------------|
| `[ExternalEvent]` on a method whose type is not `partial` (RVTTK0005)       | Mark the containing type `partial`.                             |
| `[ExternalEvent]` method returns `Task` or is `async void` (RVTTK0001/0002) | Use a synchronous signature; use `Async…` event types to await. |
| Generic `[ExternalEvent]` method (RVTTK0003)                                | Use concrete parameter types.                                   |
| Using an external event while already in API context                        | Call the API directly.                                          |
| `ExternalEvent` or `[ExternalEvent]` not found                              | The `Nice3point.Revit.Toolkit` package is not referenced.       |
