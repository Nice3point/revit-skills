---
name: configuring-dotnet-hosting
description: >
  Configure a .NET application host and its feature-registration extensions on IHostApplicationBuilder.
  USE FOR: composing HostApplicationBuilder/WebApplicationBuilder, writing IHostApplicationBuilder extension methods, wiring shared service defaults, and host startup and lifecycle.
  DO NOT USE FOR: choosing a service lifetime or a single registration (use configuring-dotnet-dependency-injection), or binding and validating an options section (use binding-and-validating-options).
license: MIT
---

# Configuring .NET Hosting

Compose a host from small, named feature extensions on `IHostApplicationBuilder`, each owning one capability and returning the builder for chaining.

## When to use

- Adding or reviewing host composition in `Program.cs` or a shared defaults project.
- Writing a `Configure…`/`Add…` extension that wires a capability into every host.
- Wiring host startup, shutdown, and lifecycle hooks.

## Workflow

### Step 1: Make each capability a builder extension

Write a generic extension over `IHostApplicationBuilder` so the same wiring composes into any host (worker, web, CLI).
Return the builder.

```csharp
public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
{
    builder.ConfigureTelemetry();
    builder.ConfigureSerialization();
    builder.ConfigureHttpClients();

    return builder;
}
```

### Step 2: Compose the host from named steps

Keep `Program.cs` a readable list of capability calls; put the wiring inside each extension.

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddFeatureFlags();
builder.Services.AddEmailSending();

var host = builder.Build();
await host.RunAsync();
```

### Step 3: Keep a feature's configuration together

Read `builder.Configuration` and branch on `builder.Environment` inside the capability's extension, and keep its options binding and service registrations in the same extension when they always change together.

### Step 4: Start long-running work as a hosted service

Register continuous or startup-bound work with `AddHostedService`; never run it inline during composition.
Create a scope inside a hosted operation before resolving scoped dependencies, and keep any loop responsive to the stopping token.

### Step 5: Verify

Build the host through its normal entry point and confirm it starts, resolves the changed capability, honors host shutdown, and stops cleanly.

## Validation

- [ ] Each capability is a builder extension returning the builder.
- [ ] `Program.cs` reads as a list of capability calls, not inline wiring.
- [ ] Environment and configuration are read through the builder.
- [ ] Hosted work observes the stopping token and shuts down cleanly.

## Common Pitfalls

| Pitfall                                                 | Correct approach                                      |
|---------------------------------------------------------|-------------------------------------------------------|
| Wiring a capability inline in `Program.cs`              | Move it into a `Configure…`/`Add…` builder extension. |
| Running startup work during composition                 | Register an `IHostedService`/`BackgroundService`.     |
| Resolving a scoped service directly in a hosted service | Create a scope with `IServiceScopeFactory` first.     |
| Extension returns `void`, breaking the chain            | Return the builder (or `IServiceCollection`).         |
