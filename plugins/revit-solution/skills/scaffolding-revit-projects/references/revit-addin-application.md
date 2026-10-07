# Revit AddIn Application Template

**Load when:** creating the host for a modular add-in that has one or more `revit-addin-module` projects.

`revit-addin-application` is not a standalone feature project.
It contains the `.addin` manifest, Revit entry point, deployment settings, launch configuration, and ribbon registration.
Limit it to startup coordination and to commands that call module functionality.
Feature business logic belongs in a module, and the service configuration the host and its modules share belongs in a `revit-servicedefaults` project.

```shell
dotnet new revit-addin-application --name MyAddin --addin application --di hosting
```

## Options

| Option    | Values and default                                  | Generated behavior                                                                |
|-----------|-----------------------------------------------------|-----------------------------------------------------------------------------------|
| `--addin` | `application` (default), `dbApplication`, `command` | Selects the manifest registration and application or startup-command entry point. |
| `--di`    | `disabled` (default), `container`, `hosting`        | Adds `Host.cs` and the selected Microsoft dependency-injection implementation.    |

The host uses WPF unless it is a DB application.
The generated host has no feature `Views` and `ViewModels` folders.
Generate those in a module instead.

## Link modules and service defaults

Generate modules and the service defaults project beside the host, and reference them from the application:

```shell
dotnet new revit-addin-module --name MyFeature
dotnet new revit-servicedefaults --name MyAddin.ServiceDefaults --di hosting
dotnet add MyAddin/MyAddin.csproj reference MyFeature/MyFeature.csproj
dotnet add MyAddin/MyAddin.csproj reference MyAddin.ServiceDefaults/MyAddin.ServiceDefaults.csproj
```

Apply the shared defaults in `Host.cs` before the host is built:

```csharp
builder.AddServiceDefaults();
```

Place `ExternalCommand` classes and `Application` ribbon registration in the host.
The host reference ensures each module is built and deployed with the add-in.

## Validation

- [ ] The application contains the `.addin` manifest, deployment, and debug launch configuration.
- [ ] Every shipping module is referenced by the application.
- [ ] A service defaults project uses the same `--di` value as the application, and the host calls `AddServiceDefaults`.
- [ ] Commands and ribbon registration remain in the application project.
