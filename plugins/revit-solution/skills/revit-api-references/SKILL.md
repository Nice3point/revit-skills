---
name: revit-api-references
description: >
  Reference the Autodesk Revit API assemblies using the Nice3point.Revit.Api.* or Nice3point toolkit, extensions or other NuGet packages.
  USE FOR: adding a Revit API assembly reference.
  DO NOT USE FOR: the version matrix and conditional compilation (use revit-multi-version-configuration).
license: MIT
---

# Revit API References

The `Nice3point.Revit.Api.*` NuGet packages carry the Revit API assemblies; a project compiles for any Revit year without that year installed locally.
A local file (`HintPath`) reference ties the build to one installed version at one path and breaks CI and teammates' machines.

## When to use

- Adding a reference to `RevitAPI`, `RevitAPIUI`, or another Revit-targeting assembly.
- Reviewing a project that references Revit DLLs by local path.

## Workflow

### Step 1: Reference the API package

```xml

<PackageReference Include="Nice3point.Revit.Api.RevitAPI" Version="$(RevitVersion).*"/>
<PackageReference Include="Nice3point.Revit.Api.RevitAPIUI" Version="$(RevitVersion).*"/>

<PackageReference Include="Nice3point.Revit.Extensions" Version="2025.0.0" Condition="$(RevitVersion) == '2025'"/>
<PackageReference Include="Nice3point.Revit.Extensions" Version="2026.0.0" Condition="$(RevitVersion) == '2026'"/>
<PackageReference Include="Nice3point.Revit.Extensions" Version="2027.0.0" Condition="$(RevitVersion) == '2027'"/>
```

The `$(RevitVersion).*` wildcard resolves the assembly set for the active configuration.
`$(RevitVersion)` is supplied by the `Nice3point.Revit.Sdk`, which derives it from the build configuration (for example `Release.R27` → `2027`) — see `revit-sdk-project-configuration`.
The `$(RevitVersion).*` wildcard can be used for built-in API dependencies, they almost never have critical changes within a major version; 
for all other dependencies, always use the exact package version to avoid breaking when the package is updated.
With central package management, pin the version in `Directory.Packages.props`.

### Step 2: Never reference a local Revit DLL

Do not add a local file reference — it requires that exact Revit version at that exact path on every build machine:

```xml
<!-- BAD -->
<Reference Include="RevitAPI">
    <HintPath>C:\Program Files\Autodesk\Revit 2027\RevitAPI.dll</HintPath>
</Reference>
```

### Step 3: Find the right package

Assemblies ship as `Nice3point.Revit.Api.*` with `RevitAPI`, `RevitAPIUI`, `RevitAPIIFC`, `AdWindows`, `UIFramework`, and more.
Check the `Nice3point.Revit.Api.*` packages before adding any assembly reference; only request a new one if it does not exist.

### Step 4: Verify

Restore and build on a machine (or CI) with no Revit installed.

## Validation

- [ ] Revit API assemblies come from `Nice3point.Revit.Api.*` packages.
- [ ] No `HintPath`/local-file reference to a Revit DLL remains.
- [ ] The version uses the `$(RevitVersion).*` wildcard (or central package management).
- [ ] The project restores and builds without Revit installed.

## Common Pitfalls

| Pitfall                                            | Correct approach                                       |
|----------------------------------------------------|--------------------------------------------------------|
| `<HintPath>` to an installed Revit DLL             | Use `Nice3point.Revit.Api.*` with `$(RevitVersion).*`. |
| A fixed API version that breaks other years        | Use the `$(RevitVersion).*` wildcard.                  |
| An external NuGet package targets a floating version | Use the exact package version.                       |
| Copying a Revit DLL into the repo                  | Reference the NuGet package.                           |
