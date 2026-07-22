# Revit AddIn Solution Template

**Load when:** a new repository needs standard source layout, ModularPipelines build automation, an MSI installer, an Autodesk App Store bundle, test orchestration, or CI.

`revit-addin-sln` creates solution infrastructure, not an add-in project.
It creates `source`, `build`, `output`, `.gitignore`, `CHANGELOG.md`, `global.json`, run configurations, and a generated README.
Create the application and modules under `source` after creating this solution.

```shell
dotnet new revit-addin-sln --name MyAddin --pipeline GitHub --bundle --includeTests
```

## Options

| Option           | Values and default                      | Generated behavior                                                                                                         |
|------------------|-----------------------------------------|----------------------------------------------------------------------------------------------------------------------------|
| `--pipeline`     | `GitHub` (default), `Azure`, `Disabled` | Adds GitHub Actions, Azure DevOps, or no CI/CD configuration. GitHub also adds changelog and GitHub-release build modules. |
| `--installer`    | `true` (default) or `false`             | Adds the `install` project and MSI creation module.                                                                        |
| `--bundle`       | `false` (default) or `true`             | Adds Autodesk App Store bundle modules and the `Bundle` section in `build/appsettings.json`.                               |
| `--includeTests` | `false` (default) or `true`             | Adds `tests`, sets a default test runner, and adds the build module that runs tests for each supported configuration. |

The `build` project uses ModularPipelines to compile each declared release configuration and produce artifacts.
`pack` produces an installer only when installer support was selected and a bundle only when bundle support was selected.

## Initialize and build

Initialize Git and make the first commit before running the build because ModularPipelines requires it.

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
- [ ] Git has an initial commit before the ModularPipelines build runs.
- [ ] The build succeeds for every declared `Release.RNN` configuration.
