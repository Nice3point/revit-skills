---
name: revit-template-migration
description: >
  Upgrade a Nice3point.Revit.Templates project or solution to the latest template or SDK version.
  USE FOR: moving a scaffolded add-in, module, benchmark, test project, or solution to a newer template or SDK version, or adding template options to an existing scaffold.
  DO NOT USE FOR: creating a brand-new project (use scaffolding-revit-projects), or changing only the supported Revit-year matrix (use revit-multi-version-configuration).
license: MIT
---

# Revit Template Migration

The dotnet template engine has no in-place upgrader.
Generate an equivalent project from the target template version, then use the diff between that generated project and the current project as the migration plan.
The generated output is the source of truth for the selected template version and options.

## When to use

- Upgrading a scaffolded add-in, module, benchmark, test project, or solution to a newer template or SDK version.
- Adding a template option such as a bundle, installer, tests, or CI to an existing scaffold.

## When not to use

- Starting a new project from scratch — that is `scaffolding-revit-projects`.
- Adding or removing a supported Revit year within an existing project — that is `revit-multi-version-configuration`.

## Workflow

### Step 1: Generate a target reference

Install a new templates version.
Create the same template type with the same name and options as your current project configured in an empty temporary directory.

```shell
dotnet new install Nice3point.Revit.Templates
dotnet new <template-short-name> --help
dotnet new <template-short-name> --name <ProjectName> --output <reference-directory> <options>
```

Match the project/solution name to avoid namespace and generated-path noise.

### Step 2: Review the generated diff

Compare the reference directory with the current project or solution.

```shell
git diff --no-index -- <current-directory> <reference-directory>
```

Treat every difference as a review item.
Take template-generated infrastructure and configuration changes that the current project needs.
Keep project identity, business logic, and deliberate product-specific customization unless the target template requires a compatible rewrite.

### Step 3: Apply the reviewed changes

Update each project only from the matching generated template type.
Keep `.addin`, GUIDs, deployment logic and dynamic-loading configuration in the original project.

### Step 4: Verify

Build every declared `Debug.RNN` and `Release.RNN` configuration.
Run every generated pipeline path selected by the project: tests, installer, bundle, or release preparation.

## Validation

- [ ] The reference uses the exact target template version, short name, name, and options.
- [ ] The generated reference was compared with the current project or solution before changes were applied.
- [ ] Only reviewed template changes were transferred.
- [ ] Project identity and business logic were preserved.
- [ ] Every declared configuration and selected output path succeeds.

## Common Pitfalls

| Pitfall                                                         | Correct approach                                                       |
|-----------------------------------------------------------------|------------------------------------------------------------------------|
| Generating the latest template when you need the target version | Install and generate the exact target version before comparing.        |
| Comparing different template options                            | Match every current option for a meaningful diff.                      |
| Copying a generated tree over the existing project              | Transfer reviewed changes selectively and preserve project-owned code. |
| Applying an application configuration to a module               | Compare each project with its own template type.                       |
