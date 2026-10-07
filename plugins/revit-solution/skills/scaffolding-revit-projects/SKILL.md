---
name: scaffolding-revit-projects
description: >
  Scaffold an Autodesk Revit add-in, service defaults, benchmark, or test project from the Nice3point.Revit.Templates.
  USE FOR: installing the templates and creating a new project or solution with dotnet new.
  DO NOT USE FOR: configuring an existing project's SDK, versions, or references (use revit-sdk-project-configuration), or upgrading a scaffolded project to a newer template version (use revit-template-migration).
license: MIT
---

# Scaffolding Revit Projects

The `Nice3point.Revit.Templates` package provides `dotnet new` templates for Revit add-ins, solutions, benchmarks, and tests, preconfigured for `Nice3point.Revit.Sdk`.
Choose the project topology before generating files.
Choosing the wrong project shape causes either a monolithic add-in or a modular add-in without a host.

These rules apply to every generated project:

1. **Template options define the project shape.**
   Generate a fresh project with the needed options; do not add generated infrastructure by hand.
2. **A `Debug.RNN` or `Release.RNN` configuration targets one Revit year.**
   Replace `NN` with the final two digits of that year; for example, `Debug.R27` targets Revit 2027.

## When to use

- Starting a new Revit add-in, benchmark, or test project or solution.

## Workflow

### Step 1: Install the templates

```shell
dotnet new install Nice3point.Revit.Templates
```

### Step 2: Select the template and its options

Match the required outcome to one reference and open only that reference.

- [references/revit-addin-solution.md](references/revit-addin-solution.md) — **Load when:** a repository needs a standard layout, default root files, ModularPipelines build, MSI installer, App Store bundle, tests, or CI.
- [references/revit-addin.md](references/revit-addin.md) — **Load when:** a small self-contained add-in needs one project that contains its manifest, entry point, and UI.
- [references/revit-addin-application.md](references/revit-addin-application.md) — **Load when:** creating the host project that contains the manifest of a modular add-in; use it with `revit-addin-module`.
- [references/revit-addin-module.md](references/revit-addin-module.md) — **Load when:** adding one modular feature, service, or WPF MVVM area to an application host.
- [references/revit-servicedefaults.md](references/revit-servicedefaults.md) — **Load when:** a modular add-in needs one project for the service registrations its application and modules share.
- [references/revit-benchmark.md](references/revit-benchmark.md) — **Load when:** measuring Revit API code with BenchmarkDotNet inside Revit.
- [references/revit-tunit.md](references/revit-tunit.md) — **Load when:** creating a TUnit test project that runs inside Revit.

### Step 3: Generate the project or solution

```shell
dotnet new revit-addin-sln --name MyAddin
```

Pass only the options that change the default shape.
For example, create a solution with an App Store bundle and tests:

```shell
dotnet new revit-addin-sln --name MyAddin --bundle --tests
```

For a modular solution, create the host, a module, and the service defaults project under `source`, then reference them from the host:

```shell
dotnet new revit-addin-application --name MyAddin --di hosting
dotnet new revit-addin-module --name MyFeature
dotnet new revit-servicedefaults --name MyAddin.ServiceDefaults --di hosting
dotnet add MyAddin/MyAddin.csproj reference MyFeature/MyFeature.csproj
dotnet add MyAddin/MyAddin.csproj reference MyAddin.ServiceDefaults/MyAddin.ServiceDefaults.csproj
```

### Step 4: Initialize the solution repository

For a solution, create add-in projects under `source` and make an initial Git commit before running its build.
ModularPipelines and GitVersion need repository history.

```shell
git init
git add .
git commit -m "Initial commit"
```

### Step 5: Verify

Restore and build a declared `Debug.RNN` configuration.
For a solution, run its ModularPipelines build from `build`.

```shell
dotnet build <ProjectPath> --configuration Debug.RNN
cd build
dotnet run
```

## Validation

- [ ] The templates are installed via `dotnet new install Nice3point.Revit.Templates`.
- [ ] The chosen template and options match the intended topology and output.
- [ ] A modular add-in has an application host, modules referenced from that host, and entry points in the host.
- [ ] Service registrations the host and its modules share are placed in a service defaults project, and the host calls `AddServiceDefaults`.
- [ ] A solution has an initial Git commit before its build runs.
- [ ] The generated project builds for one declared `Debug.RNN` or `Release.RNN` configuration.

## Common Pitfalls

| Pitfall                                                       | Correct approach                                                                                 |
|---------------------------------------------------------------|--------------------------------------------------------------------------------------------------|
| Treating an application template as a complete modular add-in | Create it with one or more modules, then add project references from the application to modules. |
| Putting commands or ribbon registration in a module           | Place Revit entry points in the application project.                                             |
| Guessing a template option                                    | Open the reference for that template and use its documented CLI option.                          |
| Building a scaffolded solution before committing              | Commit first; GitVersion needs history.                                                          |
