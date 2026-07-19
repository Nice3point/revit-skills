---
name: revit-addin-publishing
description: >
  Configure an Autodesk Revit add-in project so its build copies the add-in and .addin manifest to the local Revit or a distribution folder, using the Nice3point.Revit.Sdk publish targets.
  USE FOR: setting up a project to copy built add-in files to the Revit add-ins folder or a distribution folder, bundling extra content into the output, and relying on per-version manifest patching.
  DO NOT USE FOR: launching and debugging the add-in from the IDE (use revit-addin-debugging), dependency isolation or repacking (use revit-dependency-isolation), or packaging a versioned App Store bundle (use revit-addin-bundle).
license: MIT
---

# Revit Add-in Publishing

The `Nice3point.Revit.Sdk` targets copy the built add-in and its `.addin` manifest to one of two destinations and patch the manifest per Revit version.
`DeployAddin` copies into the local Revit add-ins folder (`%AppData%\Autodesk\Revit\Addins\<version>`), so the running Revit loads the add-in on its next start; `PublishAddin` copies into `bin\publish` (like `dotnet publish`) for distribution.
Enable them only in the project that owns the `.addin` manifest.
To launch and debug the deployed add-in from the IDE, use `revit-addin-debugging`.

## When to use

- Setting up a project to copy its add-in to the local Revit for testing, or to `bin/publish` for distribution.
- Bundling extra content (families, resources) into the add-in output.

## Workflow

### Step 1: Deploy or publish

```xml
<DeployAddin>true</DeployAddin> <!-- to %AppData%\Autodesk\Revit\Addins\<version> -->
<PublishAddin>true</PublishAddin> <!-- to bin\publish for distribution -->
```

`DeployAddin` implies publishing.
Override the targets with `AddinDeployDir` / `AddinPublishDir`.

### Step 2: Include extra content

```xml
<Content Include="Families\**" PublishDirectory="Families" CopyToPublishDirectory="PreserveNewest"/>
```

### Step 3: Rely on manifest patching

The SDK removes the `ManifestSettings` node for Revit versions older than 2026 during publish, so one `.addin` works across supported years (see `revit-dependency-isolation`).

### Step 4: Verify

Build, and confirm the add-in and manifest land in the deploy or publish folder and that Revit loads the add-in.

## Validation

- [ ] Deploy/publish is enabled only in the project holding the `.addin` manifest.
- [ ] Extra content is declared with `PublishDirectory` / `CopyToPublishDirectory`.
- [ ] The manifest works across supported years via SDK patching.

## Common Pitfalls

| Pitfall                                                     | Correct approach                                                |
|-------------------------------------------------------------|-----------------------------------------------------------------|
| Enabling deploy in a class-library project with no `.addin` | Enable it only in the manifest-owning project.                  |
| Copying families with a raw `<None>` item                   | Use `<Content … PublishDirectory=… CopyToPublishDirectory=…/>`. |
| Hand-editing the manifest per Revit version                 | Let the SDK patch `ManifestSettings` on publish.                |
