---
name: revit-assembly-resolution
description: >
  Redirect Autodesk Revit add-in assembly resolution to a plugin folder with Nice3point.Revit.Toolkit ResolveHelper.
  USE FOR: fixing a FileNotFoundException when loading a WPF window or third-party dependency Revit cannot resolve, or pinning which folder a bounded load resolves from.
  DO NOT USE FOR: entry points derived from the Toolkit base classes (ExternalCommand, ExternalApplication, ExternalDBApplication), which open a resolve scope automatically, or preventing two add-ins' dependency versions from colliding process-wide (use revit-dependency-isolation).
license: MIT
---

# Revit Assembly Resolution

Revit does not probe an add-in's folder for dependencies; loading a window or third-party assembly can throw `FileNotFoundException`.
`ResolveHelper.BeginAssemblyResolveScope` (from `Nice3point.Revit.Toolkit`) redirects resolution to a type-adjacent or explicit folder for the scope's lifetime.
Scopes nest — the innermost is searched first.

## When to use

- A dependency or WPF window fails to load with a resolution error.
- Isolating a conflicting assembly version for a bounded operation.

## When not to use

- Inside an entry point derived from the Toolkit base classes (`ExternalCommand`, `AsyncExternalCommand`, `ExternalApplication`, `ExternalDBApplication`) — they already open a resolve scope, making a manual one redundant.
- Preventing two add-ins from loading conflicting versions of the same dependency process-wide — that is `revit-dependency-isolation`.

## Workflow

### Step 1: Wrap the load in a resolve scope

```csharp
using (ResolveHelper.BeginAssemblyResolveScope<Application>())
{
    var window = new SettingsWindow();
    window.Show();
}
```

The generic and `typeof(...)` overloads probe the folder of the given type's assembly.
Pass an explicit directory instead when the dependencies live elsewhere — a shared or external libraries folder:

```csharp
using (ResolveHelper.BeginAssemblyResolveScope(@"C:\Libraries"))
{
    return LoadExternalLibrary();
}
```

### Step 2: Scope it tightly and nest for isolation

Open the scope only around the load that needs it, and nest scopes when a dependency version must be isolated from an outer one.

### Step 3: Verify

Confirm the dependency loads without a resolution error, and that resolution behavior is restored after the scope disposes.

## Validation

- [ ] The scope wraps the specific load that fails to resolve.
- [ ] Entry points rely on the base classes instead of a manual scope.
- [ ] The scope disposes, restoring normal resolution.

## Common Pitfalls

| Pitfall                                                          | Correct approach                                          |
|------------------------------------------------------------------|-----------------------------------------------------------|
| A hand-written `AppDomain.CurrentDomain.AssemblyResolve` handler | Use `ResolveHelper.BeginAssemblyResolveScope`.            |
| Adding a scope inside a Toolkit entry point                      | The base class already resolves; remove it.               |
| `ResolveHelper` not found                                        | The `Nice3point.Revit.Toolkit` package is not referenced. |
