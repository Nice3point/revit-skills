---
name: dotnet-source-generated-logging
description: >
  Write .NET logs with the LoggerMessage source generator for ILogger calls.
  USE FOR: adding or reviewing log statements built from [LoggerMessage] partial methods — message templates, log levels, event ids, exception logging, and logging scopes.
license: MIT
---

# Source-Generated Logging

Use the `LoggerMessage` source generator (part of `Microsoft.Extensions.Logging`) to declare log statements as partial methods.
Generated methods allocate nothing when the level is disabled, validate the message template against the typed parameters at compile time, and keep structured fields queryable — unlike `logger.LogInformation($"...")`, which allocates on every call and flattens the arguments into a string.

## When to use

- Adding a log call at any level (`Trace`, `Debug`, `Information`, `Warning`, `Error`, lifecycle).
- Reviewing code that calls `logger.LogInformation($"...")`, `logger.Log(...)`, or an interpolated template.
- Attaching an exception or repeated contextual fields to log output.

## Workflow

### Step 1: Mark the owning type partial

Add `partial` to the class that declares the log methods so the generator can emit their bodies.

```csharp
public sealed partial class UserService(ILogger<UserService> logger)
```

### Step 2: Declare a static partial log method per event

1. Annotate a `private static partial void` method with `[LoggerMessage]`.
2. Take `ILogger` as the first parameter.
3. Write a template whose `{Placeholder}` names match the remaining typed parameters in order.
4. Choose the level in the attribute; add an event id only when the surrounding logging policy already uses ids.
5. Pass values as parameters — never interpolate or `string.Format` the template.

```csharp
[LoggerMessage(LogLevel.Information, "User {UserId} created")]
private static partial void LogUserCreated(ILogger logger, Guid userId);
```

### Step 3: Log exceptions through the exception parameter

Add an `Exception` parameter (any position after the logger).
The generator binds it as the entry's exception instead of formatting it into the message text.

```csharp
[LoggerMessage(LogLevel.Error, "Failed to create user {UserId}")]
private static partial void LogUserCreationFailed(ILogger logger, Exception exception, Guid userId);
```

### Step 4: Add shared context with a scope

When several entries share the same context, open a `BeginScope` rather than repeating the value in every template.

```csharp
using var scope = logger.BeginScope(new Dictionary<string, object?>
{
    ["Email"] = user.Email,
    ["Phone"] = user.Phone
});
```

### Step 5: Call the generated method and build

Call the method at the log site and build once — a template/parameter mismatch is a compile error.

```csharp
LogUserCreated(logger, user.Id);
```

## Validation

- [ ] The type that declares log methods is `partial`.
- [ ] Log statements go through `[LoggerMessage]` methods, not direct `logger.Log*` calls.
- [ ] Templates use `{Placeholder}` tokens matching the typed parameters — no interpolation or `string.Format`.
- [ ] Exceptions are passed as the `Exception` parameter, not concatenated into the message.
- [ ] Repeated context uses a logging scope.

## Common Pitfalls

| Pitfall                                           | Correct approach                                                            |
|---------------------------------------------------|-----------------------------------------------------------------------------|
| `logger.LogInformation($"Created {userId}")`      | `LogUserCreated(logger, userId)` with a `[LoggerMessage]` method.           |
| Interpolating into the template                   | Named `{Placeholder}` tokens bound to typed parameters.                     |
| Formatting the exception into the message string  | Add an `Exception` parameter; the generator attaches it.                    |
| Non-`static` or non-`partial` log method          | `[LoggerMessage]` methods must be `static partial`, and the type `partial`. |
| Placeholder names or count differ from parameters | Match names and order; the build fails otherwise.                           |
