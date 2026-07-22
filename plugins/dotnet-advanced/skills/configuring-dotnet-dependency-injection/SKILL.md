---
name: configuring-dotnet-dependency-injection
description: >
  Register and review .NET dependency-injection services and lifetimes on IServiceCollection.
  USE FOR: adding a registration, choosing singleton/scoped/transient, grouping registrations behind a feature extension, assembly scanning with Scrutor, or validating a service graph.
  DO NOT USE FOR: choosing which kind of type to declare (use designing-dotnet-types), or wiring the host builder and its lifecycle (use configuring-dotnet-hosting).
license: MIT
---

# Configuring .NET Dependency Injection

Register a service in the composition that owns its lifetime, grouped with the capability it belongs to.

## When to use

- Adding or reviewing an `IServiceCollection` registration.
- Choosing a lifetime, or debugging a captive-dependency or disposal problem.
- Scanning an assembly for conventional registrations.

## Workflow

### Step 1: Register with the owning capability

Group the registration in a feature extension method that returns `IServiceCollection` for fluent chaining; do not scatter `AddX` calls across the composition root.

```csharp
public static class EmailServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEmailSenders()
        {
            services.AddSingleton<IEmailSender, SmtpEmailSender>();

            return services;
        }
    }
}
```

### Step 2: Choose the lifetime from state and concurrency

- **Singleton** — thread-safe, process-wide state, or a resource owned for the process lifetime.
- **Scoped** — one instance per operation; the host opens a scope per window, request, or endpoint.
- **Transient** — lightweight, stateless, constructed per consumer.

Never inject a scoped service into a singleton, and never make a service singleton when it holds per-operation mutable state.

### Step 3: Register an interface only for a real boundary

Register the concrete type when no consumer needs an abstraction; register `interface + implementation` when a consumer depends on an intentional seam.
Register a hosted service separately from its ordinary contract when both resolve to the same process-owned instance.

### Step 4: Scan when registrations are conventional

Use Scrutor's `Scan` to register many types by convention, not by listing each, when they share a marker interface or naming pattern.

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<SmtpEmailSender>()
    .AddClasses(classes => classes.AssignableTo<IValidator>())
    .AsImplementedInterfaces()
    .WithTransientLifetime());
```

### Step 5: Create an extension for group registration

Move every registration group — a set of related `AddX` calls or a Scrutor `Scan` — into its capability's extension method, and let the host composition root call only those methods.
Registration detail never lives at the place the host is configured.

Declare the extension with a C# 14 extension block; the `IServiceCollection` receiver is named once for the whole capability:

```csharp
public static class EmailServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEmailSenders()
        {
            services.AddSingleton<IEmailTemplateStore, FileEmailTemplateStore>();

            services.Scan(scan => scan
                .FromAssemblyOf<SmtpEmailSender>()
                .AddClasses(classes => classes.AssignableTo<IEmailSender>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            return services;
        }
    }
}
```

The composition root then reads as a flat list of capabilities, with no `AddX` or `Scan` detail inlined:

```csharp
services.AddEmailSenders();
```

### Step 6: Verify the graph

Build the host through its normal entry point and resolve the changed service through the owning path or a focused composition test.
Never construct a second `ServiceProvider` in production registration code.

## Validation

- [ ] The host that owns the behavior owns the registration.
- [ ] The lifetime matches state, concurrency, and disposal needs.
- [ ] No singleton captures a scoped or transient dependency.
- [ ] Registrations are grouped with their capability, and the normal host composition succeeds.
- [ ] The composition root calls only capability extension methods; no `AddX` group or Scrutor `Scan` is inlined where the host is configured.

## Common Pitfalls

| Pitfall                                                             | Correct approach                                                                        |
|---------------------------------------------------------------------|-----------------------------------------------------------------------------------------|
| Scoped service injected into a singleton                            | Make the consumer scoped, or pass a factory / `IServiceScopeFactory`.                   |
| Singleton holding per-request mutable state                         | Use scoped or transient.                                                                |
| `Scan` not found                                                    | The `Scrutor` package is not referenced in the project.                                 |
| A second `new ServiceProvider()` to resolve something               | Resolve through the real host or a composition test.                                    |
| `AddX` group or Scrutor `Scan` inlined where the host is configured | Wrap it in a capability extension (e.g. `AddEmailSenders`) and call that from the root. |
