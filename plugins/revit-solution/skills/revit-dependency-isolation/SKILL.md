---
name: revit-dependency-isolation
description: >
  Prevent Autodesk Revit add-in dependency conflicts with AssemblyLoadContext isolation (Revit 2027+) or ILRepack repacking (legacy).
  USE FOR: resolving crashes caused by two add-ins loading different versions of the same dependency, by isolating your add-in's assemblies.
  DO NOT USE FOR: resolving a missing dependency at load time (use revit-assembly-resolution).
license: MIT
---

# Revit Dependency Isolation

Add-ins share one process; two add-ins using different versions of the same dependency crash Revit.
Revit 2027+ isolates each add-in in its own `AssemblyLoadContext`; earlier versions need dependencies repacked into the add-in assembly.
The `Nice3point.Revit.Sdk` provides both.

## When to use

- A dependency version conflict with another add-in crashes Revit.
- Choosing between isolation (2027+) and repacking (legacy) for a distributable add-in.

## When not to use

- A single dependency simply fails to load (a `FileNotFoundException`) — use `revit-assembly-resolution`.

## Workflow

### Step 1: Isolate on Revit 2027+

Revit 2027+ isolation is declared in the `.addin` manifest.
Add a `ManifestSettings` block that gives the add-in its own load context:

```xml
<ManifestSettings>
    <UseRevitContext>False</UseRevitContext>
    <ContextName>RevitAddin</ContextName>
</ManifestSettings>
```

`UseRevitContext=False` loads the add-in in its own `AssemblyLoadContext` named by `ContextName`; it can then use any dependency version without colliding with other add-ins.
Adding `ManifestSettings` on Revit versions older than 2027 crashes Revit — the SDK strips the node during publish for older years (see `revit-addin-publishing`).

This is separate from `<EnableDynamicLoading>true</EnableDynamicLoading>` — a stock .NET SDK property every modern add-in sets; it emits the add-in's dependencies to the output folder.
That property is a prerequisite for loading dependencies at all, not the isolation mechanism (see `revit-sdk-project-configuration`).

### Step 2: Repack for legacy Revit (pre-2027)

Revit 2026 add-ins and lower without isolation, merge dependencies into the add-in with ILRepack.

```xml
<IsRepackable Condition="'$(RevitVersion)' &lt; '2027'">true</IsRepackable>
<RepackBinariesExcludes>$(AssemblyName).UI.dll;System*.dll</RepackBinariesExcludes>
```

Repacking requires the `ILRepack` package.
Prefer isolation on 2027+ over repacking.

### Step 3: Verify

Load the add-in alongside another that uses a different version of the same dependency, and confirm both load without conflict.

## Validation

- [ ] Revit 2027+ add-ins isolate with the `.addin` `ManifestSettings` block (not `EnableDynamicLoading`, which is a separate prerequisite).
- [ ] Legacy add-ins use `IsRepackable` with an appropriate excludes list.
- [ ] `ManifestSettings` is not shipped raw to Revit versions older than 2027 (the SDK patches it on publish).

## Common Pitfalls

| Pitfall                                                 | Correct approach                                                                                     |
|---------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| `ManifestSettings` shipped to Revit < 2027              | Let the SDK patch the manifest on publish; keep the block for 2027+.                                 |
| Treating `EnableDynamicLoading` as the isolation switch | It only emits dependencies to the output folder; isolate with the manifest `ManifestSettings` block. |
| Repacking a .NET Core add-in                            | Use isolation on 2027+, not ILRepack.                                                                |
| Repacking the UI or `System.*` assemblies               | Exclude them via `RepackBinariesExcludes`.                                                           |
