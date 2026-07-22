---
name: binding-and-validating-options
description: >
  Bind a configuration section to a typed options class and validate it at startup with the .NET options pattern.
  USE FOR: adding an options type, binding it from configuration, applying DataAnnotations or custom validation, and failing fast on invalid configuration with ValidateOnStart.
  DO NOT USE FOR: general host composition and the order of builder extensions (use configuring-dotnet-hosting).
license: MIT
---

# Binding and Validating Options

Represent a cohesive configuration section as an immutable options type, bound once and validated at startup; a misconfigured process fails fast, not at first use.

## When to use

- Adding configuration that a service reads (endpoints, credentials, limits, paths).
- Reviewing that required configuration is validated before the host starts.
- Replacing raw `IConfiguration["Section:Key"]` lookups scattered across services.

## When not to use

- A single value read in exactly one place, with no validation or reuse — a direct `IConfiguration` read is fine; do not model an options type for it.
- Composing the host and ordering builder extensions — that is `configuring-dotnet-hosting`.

## Workflow

### Step 1: Model one section as one options type

Use a `record` with `init` properties and DataAnnotations describing the constraints.
One type per configuration section.

```csharp
public sealed record DatabaseOptions
{
    [Required]
    public required string ConnectionString { get; init; }

    [Range(1, 100)]
    public int MinPoolSize { get; init; }

    [Range(1, 100)]
    public int MaxPoolSize { get; init; }
}
```

### Step 2: Bind and validate at registration

Bind from the named section, validate DataAnnotations, and add `ValidateOnStart` when the process cannot operate without valid values.

```csharp
builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### Step 3: Add custom validation for cross-field rules

When one field constrains another, add a `Validate` predicate or an `IValidateOptions<T>` implementation; do not encode the rule in a consumer.

```csharp
    .Validate(options => options.MinPoolSize <= options.MaxPoolSize, "MinPoolSize must not exceed MaxPoolSize");
```

### Step 4: Consume through the right interface

Inject `IOptions<T>` for singleton configuration fixed at startup, `IOptionsSnapshot<T>` for per-scope reads, or `IOptionsMonitor<T>` when values can change at runtime.
Read `.Value`; never store mutable config on a service.

### Step 5: Verify

Start the host with a missing or out-of-range required value and confirm startup fails with a clear message; start it with valid values and confirm the service reads them.

## Validation

- [ ] One options type maps to one configuration section.
- [ ] The type is immutable (`init`/`required`), with DataAnnotations for constraints.
- [ ] Required configuration uses `ValidateOnStart` to fail fast.
- [ ] Consumers inject `IOptions*<T>`, not raw `IConfiguration`.

## Common Pitfalls

| Pitfall                                                            | Correct approach                                                              |
|--------------------------------------------------------------------|-------------------------------------------------------------------------------|
| Reading `IConfiguration["Database:ConnectionString"]` in a service | Bind a `DatabaseOptions` type and inject `IOptions<DatabaseOptions>`.         |
| Invalid config discovered at first request                         | Add `ValidateDataAnnotations().ValidateOnStart()`.                            |
| Cross-field rule checked inside a consumer                         | Add `.Validate(...)` or an `IValidateOptions<T>`.                             |
| Mutating an options instance after binding                         | Keep it immutable; use `IOptionsMonitor<T>` for runtime changes.              |
| `ValidateDataAnnotations` not found                                | The `Microsoft.Extensions.Options.DataAnnotations` package is not referenced. |
