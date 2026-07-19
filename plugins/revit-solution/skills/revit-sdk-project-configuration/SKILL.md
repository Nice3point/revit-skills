---
name: revit-sdk-project-configuration
description: >
  Configure an Autodesk Revit project with the Nice3point.Revit.Sdk MSBuild SDK instead of hand-written target frameworks and usings.
  USE FOR: setting the project SDK, understanding what it derives automatically (target framework, implicit usings, language version), and deciding which csproj properties to leave to the SDK.
  DO NOT USE FOR: adding or removing a supported Revit version (use revit-multi-version-configuration), referencing the Revit API assemblies (use revit-api-references), or publishing the add-in (use revit-addin-publishing).
license: MIT
---

# Revit SDK Project Configuration

The `Nice3point.Revit.Sdk` MSBuild SDK derives Revit-project defaults from the active build configuration; you do not hand-write the target framework, language version, usings, or per-configuration build settings.

## When to use

- Creating or reviewing a Revit project's `.csproj`.
- Deciding which properties to set explicitly and which the SDK already provides.

## Workflow

### Step 1: Set the SDK

Reference the SDK, pinning the current published version:

```xml
<Project Sdk="Nice3point.Revit.Sdk/<version>">
```

### Step 2: Let the SDK resolve the Revit version

The SDK reads `$(RevitVersion)` from digits in the active configuration name — `Release.R27` and `Debug.R27` both resolve to `2027`, and a four-digit form (`Release.2027`) is used as-is.
Selecting the configuration is all it takes.
A configuration with no resolvable version fails the build with a clear error; set `$(RevitVersion)` explicitly only to override the derivation.

### Step 3: Let the SDK derive the framework and build defaults

From `$(RevitVersion)` the SDK sets everything you would otherwise hand-write per project and per configuration:

- The `TargetFramework` for each Revit version, from the official framework mappings Autodesk provides.
- `LangVersion=latest`, `Nullable=enable`, `ImplicitUsings=true`, `ImplicitRevitUsings=true`.
- `AppendTargetFrameworkToOutputPath=false` to reduce nesting when the add-in works on a single framework version.
- Per-configuration `Optimize`, `DebugSymbols`, and `DebugType` (`portable` for `Debug.*`, `none` for `Release.*`), plus the `DEBUG`/`RELEASE` constants.
- The `REVIT####` and `REVIT####_OR_GREATER` compilation symbols for multi-version branching (see `revit-multi-version-configuration`).

Leave all of these to the SDK.

### Step 4: Control implicit Revit usings

Based on the referenced assemblies, the SDK adds global usings for `Autodesk.Revit.DB`, `JetBrains.Annotations`, `Nice3point.Revit.Toolkit`, `Nice3point.Revit.Extensions` (and `.UI`), and the CommunityToolkit MVVM namespaces — so a typical file needs no `using` block.
Turn them all off with `<ImplicitRevitUsings>false</ImplicitRevitUsings>`, or drop a single one with `<Using Remove="Autodesk.Revit.DB"/>`.

### Step 5: Set the few properties in a root add-in project

The SDK does not author your `.addin` manifest — it copies and version-patches the one you write (see `revit-addin-publishing`) — and it does not set `EnableDynamicLoading`.
Enable it for the add-in registered as a root in the `.addin` manifest.
The .NET SDK then emits the runtime config and copies NuGet dependencies to the output folder, where Revit can load them:

```xml
<EnableDynamicLoading>true</EnableDynamicLoading>
```

### Step 6: Verify

Restore and build; confirm the target framework and the emitted `REVIT####` symbols match the active configuration.

## Validation

- [ ] The project sets `Sdk="Nice3point.Revit.Sdk/…"`.
- [ ] The target framework, language version, and usings are left to the SDK, not hand-written.
- [ ] Implicit usings are adjusted through SDK properties, not a manual using list.
- [ ] `EnableDynamicLoading` is set (the SDK does not set it for you).

## Common Pitfalls

| Pitfall                                                | Correct approach                                                                 |
|--------------------------------------------------------|----------------------------------------------------------------------------------|
| Hand-writing `<TargetFramework>` per version           | Let the SDK derive it from `RevitVersion`.                                       |
| Configuration name with no version number              | Add the year (`Release.R27`); the SDK errors when `RevitVersion` cannot resolve. |
| Re-declaring `Nullable`/`LangVersion`/`ImplicitUsings` | The SDK already sets them.                                                       |
| Removing `AppendTargetFrameworkToOutputPath=false`     | Keep it; add-in publishing depends on the flat output path.                      |
| SDK not found on restore                               | Pin an available `Nice3point.Revit.Sdk` version in the `Sdk` attribute.          |
