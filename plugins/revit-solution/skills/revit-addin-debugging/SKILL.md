---
name: revit-addin-debugging
description: >
  Configure an Autodesk Revit add-in project's IDE launch of the matching Revit with the debugger attached, using the Nice3point.Revit.Sdk launch properties.
  USE FOR: setting up a project where a debug session launches the matching Revit and breaks in the add-in, overriding the Revit path or start arguments, and preserving Hot Reload while debugging.
  DO NOT USE FOR: copying built files to the Revit add-ins folder (use revit-addin-publishing), dependency isolation or repacking (use revit-dependency-isolation).
license: MIT
---

# Revit Add-in Debugging

The `Nice3point.Revit.Sdk` makes the IDE's start-debugging action launch Revit and attach the debugger, without a `launchSettings.json`.
Deploy the add-in first (`revit-addin-publishing`); the launched Revit loads the deployed build.

## When to use

- Setting up a project where a debug session starts the right Revit version and breaks in the add-in.
- Pointing the launcher at a non-default Revit install or start arguments.
- Preserving Hot Reload during iterative debugging.

## When not to use

- The build never needs to run under a debugger — plain `DeployAddin` copying is enough (`revit-addin-publishing`).

## Workflow

### Step 1: Enable launch

```xml
<LaunchRevit>true</LaunchRevit>
```

The SDK sets `StartAction=Program`, `StartProgram` to `C:\Program Files\Autodesk\Revit $(RevitVersion)\Revit.exe`, and `StartArguments=/language ENG`; the IDE reads them, starts Revit, and attaches the debugger.
Enable it alongside `DeployAddin` in the project that contains the `.addin` manifest.
Each build deploys the add-in before launch.

### Step 2: Override the target when the defaults are wrong

```xml
<StartProgram>D:\Autodesk\Revit $(RevitVersion)\Revit.exe</StartProgram>
<StartArguments>/language CHS</StartArguments>
```

Set these when Revit is installed off the default path, or when forcing a language or opening a model on start.

### Step 3: Preserve Hot Reload

With `DeployAddin`, repacking (`IsRepackable`) replaces the deployed add-in assembly with a merged assembly on every build.
The loaded assembly then differs from the compiled one, Hot Reload stops applying edits, and the edit–run loop slows down.
For local debugging on Revit 2027+, disable `IsRepackable` and use manifest-level isolation (`revit-dependency-isolation`); reserve repacking for release builds on pre-2027 versions.

### Step 4: Verify

Set a breakpoint in a command, start a debug session, and confirm the matching Revit launches, loads the add-in, and stops at the breakpoint.

## Validation

- [ ] `LaunchRevit` is enabled alongside `DeployAddin` in the project that contains the `.addin` manifest.
- [ ] `StartProgram`/`StartArguments` are overridden only when the defaults do not fit.
- [ ] `IsRepackable` is off for debug builds; isolation covers dependency conflicts.

## Common Pitfalls

| Pitfall                                              | Correct approach                                                             |
|------------------------------------------------------|------------------------------------------------------------------------------|
| Looking for a `launchSettings.json`                  | The SDK defines the launch in `LaunchRevit`/`StartProgram`/`StartArguments`. |
| A debug session starts Revit but the add-in is stale | Enable `DeployAddin`; the build deploys before launch.                       |
| Hot Reload does nothing while repacking is on        | Disable `IsRepackable` for debug; isolate dependencies via the manifest.     |
| The wrong Revit year launches                        | `StartProgram` uses `$(RevitVersion)`; select the matching configuration.    |
