# Revit AddIn Template

**Load when:** a small Revit add-in needs one project that owns the `.addin` manifest, entry point, commands, user interface, local deployment, and debugging.

`revit-addin` is the single-project shape.
It generates an SDK-style project with `DeployAddin`, `LaunchRevit`, and `EnableDynamicLoading` configured for a manifest-owning add-in.
It references `Nice3point.Revit.Toolkit`, `Nice3point.Revit.Extensions`, and `Nice3point.Revit.Api.RevitAPI`.
All variants except a DB application also reference `Nice3point.Revit.Api.RevitAPIUI`.

```shell
dotnet new revit-addin --name MyAddin --addin application --wpf --di hosting --logger
```

## Options

| Option     | Values and default                                  | Generated behavior                                                                                                                                           |
|------------|-----------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `--addin`  | `application` (default), `dbApplication`, `command` | Selects the manifest registration and entry point. Application and DB application generate `Application.cs`. Command generates `Commands/StartupCommand.cs`. |
| `--wpf`    | `true` (default) or `false`                         | Adds `UseWPF`, CommunityToolkit.Mvvm, and starter `Models`, `Views`, and `ViewModels`. It does not apply to a DB application.                                |
| `--di`     | `disabled` (default), `container`, `hosting`        | Adds `Host.cs` and either Microsoft.Extensions.DependencyInjection or Microsoft.Extensions.Hosting.                                                          |
| `--logger` | `false` (default) or `true`                         | Adds Serilog packages. With DI the host configures logging; without DI the application initializes a debug logger directly.                                  |

## Choosing the add-in type

Use `application` for a normal ribbon-driven add-in.
The generated application creates the ribbon and registers the starter command.
Use `command` when the manifest must register one command directly.
Use `dbApplication` when the entry point is a database application and does not need the Revit UI API or WPF output.

## Validation

- [ ] The project contains its `.addin` manifest and the selected entry point.
- [ ] WPF, DI, and logging options match the required runtime behavior.
- [ ] A declared `Debug.RNN` configuration deploys and starts the intended Revit year.
