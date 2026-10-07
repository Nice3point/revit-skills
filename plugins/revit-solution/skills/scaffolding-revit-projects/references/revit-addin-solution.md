# Revit AddIn Solution Template

**Load when:** a new repository needs standard source layout, ModularPipelines build automation, an MSI installer, an Autodesk App Store bundle, test orchestration, or CI.

`revit-addin-sln` creates solution infrastructure, not an add-in project.
It creates `source`, `build`, `.gitignore`, `.gitattributes`, `.editorconfig`, `CHANGELOG.md`, `global.json`, run configurations, and a generated README.
Create the application and modules under `source` after creating this solution.

```shell
dotnet new revit-addin-sln --name MyAddin --pipeline GitHub --bundle --tests
```

## Options

| Option        | Values and default                      | Generated behavior                                                                                                         |
|---------------|-----------------------------------------|----------------------------------------------------------------------------------------------------------------------------|
| `--pipeline`  | `GitHub` (default), `Azure`, `Disabled` | Adds GitHub Actions, Azure DevOps, or no CI/CD configuration. GitHub also adds changelog and GitHub-release build modules. |
| `--installer` | `true` (default) or `false`             | Adds the `installer` project, the MSI creation module, and the `Installer` section in `build/appsettings.json`.            |
| `--bundle`    | `false` (default) or `true`             | Adds the Autodesk App Store bundle module and the `Bundle` section in `build/appsettings.json`.                            |
| `--tests`     | `false` (default) or `true`             | Adds `tests`, sets the Microsoft Testing Platform runner, and adds the build module that runs tests for each configuration. |

The `build` project uses ModularPipelines to compile each declared release configuration and produce artifacts in `output`.
`pack` produces an installer only when installer support was selected and a bundle only when bundle support was selected.

## Configure the installer

The build writes a manifest with the published add-in of every Revit version, and the `installer` project builds a per-user and a per-machine MSI package from it.
Set a new GUID to `UpgradeCode` in the `Installer` section of `build/appsettings.json`, and reuse it for every release of the add-in:

```json
"Installer": {
  "UpgradeCode": "3F2504E0-4F89-11D3-9A0C-0305E82C3301"
}
```

A release with the same upgrade code upgrades the installed product, and a release with a new code is installed side by side with it.

## Initialize and build

Initialize Git and make the first commit before running the build.
ModularPipelines requires repository history.

```shell
git init
git add .
git commit -m "Initial commit"
cd build
dotnet run
```

## Validation

- [ ] Add-in projects are under `source`.
- [ ] Pipeline, installer, bundle, and tests match the selected options.
- [ ] With the installer, `UpgradeCode` contains a GUID unique to the add-in.
- [ ] Git has an initial commit before the ModularPipelines build runs.
- [ ] The build succeeds for every declared `Release.RNN` configuration.
