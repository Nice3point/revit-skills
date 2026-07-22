# Revit Test (TUnit) Template

**Load when:** creating automated tests that need Revit API access.

`revit-tunit` creates an executable test project.
It references `Nice3point.TUnit.Revit`, TUnit, `Nice3point.Revit.Api.RevitAPI`, and Polyfill.
The generated project contains a starter test and test configuration.

```shell
dotnet new revit-tunit --name MyAddin.Tests
```

The template has no options.
Keep tests in a dedicated project.
When the repository was created with `revit-addin-sln --includeTests`, place the project under the `tests` folder.

## Validation

- [ ] The test project builds for the selected `Debug.RNN` or `Release.RNN` configuration.
- [ ] Tests that touch the Revit API execute in the Revit process.
- [ ] A solution with test support places test projects under `tests`.
