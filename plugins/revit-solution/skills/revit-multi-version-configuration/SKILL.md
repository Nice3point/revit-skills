---
name: revit-multi-version-configuration
description: >
  Support multiple Autodesk Revit versions in one project with Debug.RNN/Release.RNN configurations and REVIT#### conditional compilation.
  USE FOR: declaring the supported-version matrix, adding or removing a version, and writing #if REVIT####_OR_GREATER branches for genuine API differences.
  DO NOT USE FOR: what the SDK derives per version (use revit-sdk-project-configuration), or referencing the Revit API packages (use revit-api-references).
license: MIT
---

# Revit Multi-Version Configuration

A Revit project builds one configuration per supported year.
The `Nice3point.Revit.Sdk` derives the framework and the cumulative `REVIT####` / `REVIT####_OR_GREATER` symbols from the active `Debug.RNN` / `Release.RNN` configuration.

## When to use

- Declaring or changing which Revit years a project supports.
- Writing version-specific code for a real API difference.

## When not to use

- Understanding what the SDK sets automatically per version — use `revit-sdk-project-configuration`.
- Adding the Revit API assembly references — use `revit-api-references`.

## Workflow

### Step 1: Declare the configuration matrix

```xml
<Configurations>Debug.R25;Debug.R26;Debug.R27</Configurations>
<Configurations>$(Configurations);Release.R25;Release.R26;Release.R27</Configurations>
```

Mirror the same `Debug.RNN`/`Release.RNN` set in the .slnx/.sln file.

### Step 2: Add or remove a version

Add or remove the matching `Debug.RNN` / `Release.RNN` in the solution and the project's `<Configurations>`.
For a newly released year, first update the SDK; set `TargetFramework` explicitly only when the SDK is not released with the latest Revit version support or you are using the preview API package:

```xml
<TargetFramework Condition="$(RevitVersion) == '2027'">net10.0-windows7.0</TargetFramework>
```

### Step 3: Add preprocessor directives for incompatible API

Write shared code first.
Add a `#if` branch only where the Revit API genuinely differs; keep both branches independently compilable.

```csharp
#if REVIT2024_OR_GREATER
    // API available from Revit 2024 onward
#else
    // legacy API
#endif
```

### Step 4: Verify

Restore and build every declared configuration, and run version-specific tests for each branch that changed.

## Validation

- [ ] The active solution matrix selects the intended project `Debug.RNN`/`Release.RNN` configurations.
- [ ] `#if` branches cover only genuine API differences, and both sides compile.
- [ ] Every declared configuration restores and builds.

## Common Pitfalls

| Pitfall                                             | Correct approach                                  |
|-----------------------------------------------------|---------------------------------------------------|
| Editing configurations through the IDE dialog       | Edit `<Configurations>` in the `.csproj` by hand. |
| A `#if` around code that is identical on both sides | Keep it shared; branch only on real differences.  |
| Solution and project matrices drifting apart        | Keep both lists identical.                        |
| A branch that only compiles for one version         | Ensure both `#if` sides build.                    |
