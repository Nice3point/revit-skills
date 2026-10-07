# Revit Service Defaults Template

**Load when:** a modular add-in needs one place for the service registrations its application and modules share.

`revit-servicedefaults` creates a class library on the model of the .NET Aspire service defaults project.
It holds every service registration the application and its modules share, and one `AddServiceDefaults` method applies them.
The template provides logging on Microsoft.Extensions.Logging and the log of unhandled `AppDomain` exceptions.
Serialization, HTTP clients, options, and every other shared registration are added to the same project.

```shell
dotnet new revit-servicedefaults --name MyAddin.ServiceDefaults --di hosting
```

## Option

| Option | Values and default                | Generated behavior                                                                                                    |
|--------|-----------------------------------|-----------------------------------------------------------------------------------------------------------------------|
| `--di` | `hosting` (default) or `container` | Declares `AddServiceDefaults` on `IHostApplicationBuilder` for `hosting` and on `IServiceCollection` for `container`. |

Choose the `--di` value of the application.
`AddServiceDefaults` is declared in the namespace of the host builder, and a call needs no extra `using` directive.

## Apply the defaults

Reference the project from the application, and from a module that consumes a shared type, such as a serializer context.

With `hosting`, apply the defaults in `Host.cs` before the host is built:

```csharp
builder.AddServiceDefaults();
```

With `container`, apply the defaults to the service collection and start the exception log after the provider is built:

```csharp
services.AddServiceDefaults();

_serviceProvider = services.BuildServiceProvider();
_serviceProvider.GetRequiredService<AppDomainExceptionsHandler>().LogExceptions();
```

## Validation

- [ ] The project uses the `--di` value of the application.
- [ ] The application references the project and calls `AddServiceDefaults`.
- [ ] A registration that the application and its modules share is placed in this project, not duplicated per project.
- [ ] With `container`, the host calls `LogExceptions` after the service provider is built.
