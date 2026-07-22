---
name: revit-addin-debugging
description: >
  Configure an Autodesk Revit add-in project's IDE launch of the matching Revit with the debugger attached, using the Nice3point.Revit.Sdk launch properties.
  USE FOR: setting up a project where a debug session launches the matching Revit and breaks in the add-in, overriding the Revit path or start arguments, and keeping Hot Reload responsive while debugging.
  DO NOT USE FOR: copying built files to the Revit add-ins folder (use revit-addin-publishing), dependency isolation or repacking (use revit-dependency-isolation).
license: MIT
---

# Revit Add-in Debugging

The `Nice3point.Revit.Sdk` makes the IDE's start-debugging action launch Revit and attach the debugger, without a `launchSettings.json`.
Deploy the add-in first (`revit-addin-publishing`); the launched Revit loads the build you are debugging.

## When to use

- Setting up a project where a debug session starts the right Revit version and breaks in your add-in.
- Pointing the launcher at a non-default Revit install or start arguments.
- Keeping iterative debugging and Hot Reload responsive.

## When not to use

- The build never needs to run under a debugger — plain `DeployAddin` copying is enough (`revit-addin-publishing`).

## Workflow

### Step 1: Enable launch

```xml

<LaunchRevit>true</LaunchRevit>
```

The SDK sets `StartAction=Program`, `StartProgram` to `C:\Program Files\Autodesk\Revit $(RevitVersion)\Revit.exe`, and `StartArguments=/language ENG`; the IDE reads them, starts Revit, and attaches the debugger.
Enable it in the manifest-owning project alongside `DeployAddin`; each build deploys before launch.

### Step 2: Override the target when the defaults are wrong

```xml

<StartProgram>D:\Autodesk\Revit $(RevitVersion)\Revit.exe</StartProgram>
<StartArguments>/language CHS</StartArguments>
```

Set these when Revit is installed off the default path, or when forcing a language or opening a model on start.

### Step 3: Keep Hot Reload working

Repacking (`IsRepackable`) merges dependencies as a post-build step that rewrites the output assembly; it defeats Hot Reload and slows the edit–run loop.
For local debugging on Revit 2027+, leave `IsRepackable` off and rely on manifest-level isolation (`revit-dependency-isolation`); reserve repacking for release builds on pre-2027 versions.

### Step 4: Verify

Set a breakpoint in a command, start a debug session, and confirm the matching Revit launches, loads the add-in, and stops at the breakpoint.

## Validation

- [ ] `LaunchRevit` is enabled in the manifest-owning project, alongside `DeployAddin`.
- [ ] `StartProgram`/`StartArguments` are overridden only when the defaults do not fit.
- [ ] `IsRepackable` is off for debug builds; isolation covers dependency conflicts.

## Common Pitfalls

| Pitfall                                              | Correct approach                                                             |
|------------------------------------------------------|------------------------------------------------------------------------------|
| Looking for a `launchSettings.json`                  | The SDK drives launch through `LaunchRevit`/`StartProgram`/`StartArguments`. |
| A debug session starts Revit but the add-in is stale | Enable `DeployAddin`; the build deploys before launch.                       |
| Hot Reload does nothing while repacking is on        | Disable `IsRepackable` for debug; isolate dependencies via the manifest.     |
| The wrong Revit year launches                        | `StartProgram` uses `$(RevitVersion)`; select the matching configuration.    |
