# Revit AddIn Application Template

**Load when:** creating the host for a modular add-in that has one or more `revit-addin-module` projects.

`revit-addin-application` is not a standalone feature project.
It owns the `.addin` manifest, Revit entry point, deployment settings, launch configuration, and ribbon registration.
Keep it small: coordinate startup and call module functionality from its commands instead of putting feature business logic in the host.

```shell
dotnet new revit-addin-application --name MyAddin --addin application --di container
```

## Options

| Option     | Values and default                                  | Generated behavior                                                                                                          |
|------------|-----------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------|
| `--addin`  | `application` (default), `dbApplication`, `command` | Selects the manifest registration and application or startup-command entry point.                                           |
| `--di`     | `disabled` (default), `container`, `hosting`        | Adds `Host.cs` and the selected Microsoft dependency-injection implementation.                                              |
| `--logger` | `false` (default) or `true`                         | Adds Serilog packages. With DI the host configures logging; without DI the application initializes a debug logger directly. |

The host uses WPF unless it is a DB application, but it intentionally does not generate feature `Models`, `Views`, and `ViewModels` folders.
Generate those in a module instead.

## Link modules

Generate modules beside the host and reference them from the application:

```shell
dotnet new revit-addin-module --name MyFeature
dotnet add MyAddin/MyAddin.csproj reference MyFeature/MyFeature.csproj
```

Keep `ExternalCommand` classes and `Application` ribbon registration in the host.
The host reference ensures each module is built and shipped with the add-in.

## Validation

- [ ] The application owns the `.addin` manifest, deployment, and debug launch configuration.
- [ ] Every shipping module is referenced by the application.
- [ ] Commands and ribbon registration remain in the application project.
