# Revit AddIn Module Template

**Load when:** adding one feature, reusable Revit service, domain area, or optional WPF MVVM screen to a modular application host.

`revit-addin-module` is a library project for a modular add-in.
It has Revit API, Toolkit, and Extensions references, but it has no `.addin` manifest, deploy target, launch configuration, or Revit entry point.
Create it with `revit-addin-application`, then add a project reference from that application.

```shell
dotnet new revit-addin-module --name MyFeature --wpf false
```

## Option

| Option  | Values and default          | Generated behavior                                                                                                                                       |
|---------|-----------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------|
| `--wpf` | `true` (default) or `false` | With `true`, adds `UseWPF`, CommunityToolkit.Mvvm, and starter `Models`, `Views`, and `ViewModels`. With `false`, creates an empty code-oriented module. |

Use a WPF module for a feature that owns a dialog or view model.
Use a non-WPF module for services, data access, helpers, or other feature logic without a view.
Modules can reference lower-level modules, but keep the application as the composition root.

## Validation

- [ ] The module is referenced by the manifest-owning application.
- [ ] The module does not contain an `.addin` manifest, deploy, or launch settings.
- [ ] WPF is enabled only when the module owns a UI.
