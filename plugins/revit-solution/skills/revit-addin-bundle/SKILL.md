---
name: revit-addin-bundle
description: >
  Package a versioned Autodesk Revit add-in as an Autodesk App Store bundle with the Nice3point.Revit.Templates ModularPipelines build.
  USE FOR: setting the bundle vendor metadata and version, and running the build to produce a versioned .bundle with per-Revit-year contents for App Store distribution.
  DO NOT USE FOR: per-build deploy or publish to a local Revit (use revit-addin-publishing), or launching and debugging (use revit-addin-debugging).
license: MIT
---

# Revit Add-in Bundle

An Autodesk App Store add-in ships as a `.bundle` — a `PackageContents.xml` plus a `Contents/<year>` folder for each supported Revit version.
A solution scaffolded from the templates with bundle support carries a ModularPipelines `build` project whose `pack` command assembles the bundle from every version's publish output and stamps it with a version.
This skill configures and runs that packaging; a solution created without bundle support needs the bundle wiring added first (see Common Pitfalls).

## When to use

- Packaging an add-in as an Autodesk App Store bundle.
- Setting the vendor metadata and version that go into the bundle.

## Workflow

### Step 1: Set vendor metadata

Set the vendor fields in the `Bundle` section of `build/appsettings.json` (`VendorName` is required):

```json
"Bundle": {
"VendorName": "Acme Corporation",
"VendorUrl": "https://acme.com",
"VendorEmail": "support@acme.com"
}
```

### Step 2: Resolve the version

`GitVersion` derives the bundle's `AppVersion` from Git tags; a fixed `Version` in `build/appsettings.json` overrides it.
Tag the commit before packing for a meaningful version, and commit at least once — GitVersion needs history.

### Step 3: Build the bundle

Run the `pack` command from the `build` directory.
It compiles every supported Revit configuration, then assembles the bundle from each configuration's publish output.
The same run also produces an installer only when the solution was scaffolded with installer support:

```shell
cd build
dotnet run -c Release -- pack
```

The result is a zipped `.bundle` under `output/`, laid out per version:

```text
RevitAddIn.bundle/
  PackageContents.xml
  Contents/
    2025/
      RevitAddIn.addin
      RevitAddIn/RevitAddIn.dll
    2026/
      RevitAddIn.addin
      RevitAddIn/RevitAddIn.dll
```

The generated `PackageContents.xml` carries the product name, `AppVersion`, the vendor metadata, and one component entry per Revit year.

### Step 4: Verify

Confirm the bundle has a `Contents/<year>` folder for every supported version and that `PackageContents.xml` shows the expected version and vendor fields, then upload it to the Autodesk App Store.

## Validation

- [ ] `VendorName` (at minimum) is set in the `Bundle` section of `build/appsettings.json`.
- [ ] The version derives from a Git tag, or an explicit `Version`.
- [ ] The bundle carries a `Contents/<year>` folder for each supported Revit version.

## Common Pitfalls

| Pitfall                                              | Correct approach                                                                                                                                                                                          |
|------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Building before any Git commit                       | Commit first; GitVersion needs history.                                                                                                                                                                   |
| Hand-numbering versions                              | Tag the commit and let GitVersion derive the version.                                                                                                                                                     |
| Shipping a bundle without vendor metadata            | Set the `Bundle` fields in `build/appsettings.json`.                                                                                                                                                      |
| Packing a solution scaffolded without bundle support | First create the solution with bundle support (`dotnet new revit-addin-sln --bundle`) and transfer the bundle infrastructure files from a throwaway `--bundle` solution (see `revit-template-migration`). |
| A Revit year missing from the bundle                 | Pack builds every configured version; add the year to the configuration matrix (`revit-multi-version-configuration`).                                                                                     |
