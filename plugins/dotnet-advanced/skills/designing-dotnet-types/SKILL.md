---
name: designing-dotnet-types
description: >
  Choose the right kind of .NET type for new behavior — static class, extension method, injected service, hosted service, options type, record, or interface.
  USE FOR: deciding how to shape a new unit of behavior or data before writing it, or reviewing that an existing type has the right shape for its dependencies and lifecycle.
  DO NOT USE FOR: registering a type or choosing its DI lifetime (use configuring-dotnet-dependency-injection).
license: MIT
---

# Designing .NET Types

Pick the type from its dependency and lifecycle needs, not from habit.

## When to use

- Adding a new unit of behavior and deciding whether it is a static helper, an extension, a service, or a hosted service.
- Adding data or configuration and deciding between a `record` and an options type.
- Reviewing whether a type hides dependencies it should declare.

## Static classes

Use a static class only for stateless behavior with no injected dependency, configuration, logging, I/O, clock, cache, or mutable state.
Keep a static method deterministic for the same input unless its name says otherwise.

## Extension methods

Use an extension method for a small operation centered on its receiver type, free of injected dependencies, I/O, hidden mutation, and expensive enumeration.
Reach for an ordinary service the moment the operation needs configuration, logging, another collaborator, or a lifetime.

## Services

Use an injected service when behavior depends on configuration, logging, I/O, a client, a clock, a cache, or another injectable collaborator.

- Keep the service API focused on one capability boundary.
- Use a primary constructor for dependencies; do not add fields that only mirror them.
- Add an interface when a consumer must substitute the implementation, choose among implementations, or depend on a stable boundary — not merely because the type is DI-registered.

## Hosted services

Use `IHostedService` or `BackgroundService` for work that starts with the host, stops with the host, or continuously consumes an independent source.
Keep request handling and one-shot operations in ordinary services.

## Data and configuration types

Use a `record` for immutable data crossing a method, process, or configuration boundary.
Use a dedicated options type for a cohesive configuration section; never store mutable configuration on a service.

## Validation

- [ ] The type matches its dependency and lifecycle needs.
- [ ] A static or extension method hides no I/O, mutation, or injected collaborator.
- [ ] A service exposes one capability boundary.
- [ ] An interface exists only for a real substitution or boundary need.

## Common Pitfalls

| Pitfall                                               | Correct approach                                                       |
|-------------------------------------------------------|------------------------------------------------------------------------|
| Static helper that quietly reads a file or clock      | Make it a service with the dependency injected.                        |
| Extension method taking a `DbContext` or `HttpClient` | Use a service; extensions are for receiver-centric operations.         |
| Interface added for every DI-registered class         | Register the concrete type; add an interface only for a real boundary. |
| Mutable "settings" service holding config             | Bind an options type (use binding-and-validating-options).             |
