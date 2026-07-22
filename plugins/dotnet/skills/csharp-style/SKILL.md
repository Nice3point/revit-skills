---
name: csharp-style
description: >
  Write or review C# code.
  USE FOR: any C# you write or review.
  DO NOT USE FOR: prose, markdown, or wiki text (use technical-writing), or XML doc comment content (use writing-xml-doc-comments).
license: MIT
---

# C# Style

Hold the code to an enterprise production standard, never the level of tutorial or learning material.
Write in a strict, technical style: no explanatory scaffolding, no didactic comments.

## Naming and layout

- Give every identifier its full domain meaning; never use a single-character or abbreviated name.
- Suffix a method returning `Task` or `Task<T>` with `Async`.
- Write every method with a block body, never an expression-bodied method (`=>`); an expression body is fine only for a trivial property or indexer accessor.
- Expose data through properties; never public or protected fields.
- State access modifiers explicitly; seal a class unless it is designed for inheritance.
- Use file-scoped namespaces; declare one top-level type per file.
- Collect common namespaces into global usings.

## Language features

- Use the `var` keyword to declare variables.
- Use the newest language features, patterns, types.
- Use `nameof` instead of a string literal for a member or parameter name.
- Use target-typed `new()` when the type is clear from the context.
- Use raw string literals (`"""`) for multi-line or quote-heavy text, and interpolation elsewhere.
- Use primary constructors to capture dependencies and simple state.
- Use `required` members instead of a constructor whose only job is to force initialization.
- Use range and index operators (`^`, `..`) for slicing.
- Use `static` local functions and lambdas to avoid unintended captures.
- Use `is null` or `is not null` for null checks; never the empty property pattern `is { }` as a null check.
- Use separate `if` blocks for `if-return` conditions; don't list all the conditions within a single `if` block.
- Use pattern matching, collection expressions, switch expressions.
- Use declaration pattern to check the run-time type of an expression and, if a match succeeds, assign an expression result to a declared variable.
- Use type pattern to check the run-time type of an expression.
- Use constant pattern to test that an expression result equals a specified constant.
- Use relational patterns to compare an expression result with a specified constant.
- Use logical patterns to test that an expression matches a logical combination of patterns.
- Use property pattern to test that an expression's properties or fields match nested patterns.
- Use positional pattern to deconstruct an expression result and test if the resulting values match nested patterns.
- Use var pattern to match any expression and assign its result to a declared variable.
- Use discard pattern to match any expression.
- Use list patterns to test that a sequence of elements matches corresponding nested patterns.
- Use source generators from System and external libraries; `ObservableProperty`, `LoggerMessage`, `JsonSerializerContext` and others.
- Use the `System.Memory` features and types, if this doesn't reduce code readability.
- Use `Polyfill` when the project targets multiple frameworks; it brings new types and methods to legacy targets without copying .NET sources.

## Nullability

- Use nullable types; keep public and internal contracts null-safe.
- Use `= null!` suppression if you are 100% sure the value can't be null.

## Annotations

- Express contracts with annotations from the JetBrains and `System.Diagnostics.CodeAnalysis` sets — both are large; reach for whichever fits, not a fixed few.
- Use `[Pure]` if the method doesn't make any observable state changes.
- Use `[NotNullWhen]` on `Try`-style methods with an `out` nullable result.
- Use `[PublicAPI]` to mark publicly available APIs that should not be removed and therefore should never be reported as unused.
- Use `[UsedImplicitly]` to mark a symbol as used implicitly (via reflection, in an external library, and so on).
- Use `[MustUseReturnValue]`, `[MemberNotNull]`, `[DoesNotReturnIf]`, `[StringSyntax]` and others if applicable.

## Asynchronous code

- Use `Task`/`Task<T>`; reserve `async void` for a framework-required event handler.
- Flow `CancellationToken` through I/O, broker, storage, and long-running work.
- Never block an async flow with `.Result`, `.Wait()`, or synchronous sleeps.

## Data contracts

- Use `record` for DTOs, message contracts, and configuration-style data.
- Use `init` properties if applicable.
- Use the `[Serializable]` attribute for external or serializable DTOs.

## Error handling

- Guard a public method's arguments at entry; throw `ArgumentNullException`, `ArgumentException`, or `ArgumentOutOfRangeException`.
- Throw the most specific exception type; never throw `Exception`, `SystemException`, or `ApplicationException` directly.
- Rethrow with `throw;`; never `throw exception;`.

## Performance

- Do not use deep optimization if it affects code readability.
- Use `Span` if it avoids allocations without significant code changes.
- Use `struct` for internal value types on a hot path to avoid allocations; keep them inside the owning type and don't expose them across a public boundary.
- Dispose owned streams and pooled resources.
- Unsubscribe from events depending on the object lifetime.
- Use source-generated types to avoid extra allocations or reflection overhead.

## Comments

- Comment the intent, constraint, or invariant the code cannot show; add none when the code already states it.
- Judge every comment as final standalone text: the reader has only the code as it stands, with no previous version, diff, or request to compare against; drop any comment that merely narrates the change.
- State facts in the present indicative; never argue why the code is as it is.
- Cut every purpose, result, cause, or comparison clause (`so`, `that makes`, `which makes`, `because`, `rather than`); if the clause states a fact the reader needs, make it its own sentence.

## Review

- [ ] Identifiers carry full domain meaning; async methods end in `Async`.
- [ ] Modern language features are used: `var`, pattern matching, collection and switch expressions, and source generators over hand-written equivalents.
- [ ] Nullability is explicit and annotations match real contracts.
- [ ] Async paths flow `CancellationToken` and never block.
- [ ] Shared data contracts are immutable `record` types; serializable DTOs carry `[Serializable]`.
- [ ] Exceptions are specific types, arguments are guarded at entry, and rethrows use `throw;`.
- [ ] Hot paths avoid needless allocations (`struct`, `Span`); owned disposables and event subscriptions are released on the owner's lifetime.
- [ ] Comments state facts about the code as it stands; none narrates the edit or argues why.
