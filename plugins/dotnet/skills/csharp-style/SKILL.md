---
name: csharp-style
description: >
  Write or review C# style — naming, modern language features, nullability and annotations, async, immutable data contracts, error handling, and performance.
  USE FOR: any C# you write or review.
  DO NOT USE FOR: prose, markdown, or wiki text (use technical-writing), or XML doc comment content (use writing-xml-doc-comments).
license: MIT
---

# C# Style

## Naming and layout

- Give every identifier its full domain meaning; never use a single-character or abbreviated name.
- Suffix a method returning `Task` or `Task<T>` with `Async`.
- Use file-scoped namespaces.
- Order members: private fields, constructor, public properties, public methods, private methods.

## Language features

- Use the newest language feature that makes intent clearer without hiding control flow or allocation.
- Use `is null` / `is not null` for null checks; never the empty property pattern `is { }` as a null check.
- Combine predicates over one expression with `and`/`or`/`not` patterns; parenthesize when precedence is not obvious.
- Use collection expressions (`[…]`) for clear collection literals.

## Nullability and annotations

- Enable nullable reference types; keep public and internal contracts null-safe.
- Express real, IDE- or compiler-visible contracts with annotations from the JetBrains and `System.Diagnostics.CodeAnalysis` sets — both are large, so reach for whichever fits rather than a fixed few.
- Apply `[Pure]` to side-effect-free members and `[NotNullWhen]` to `Try`-style methods with an `out` result as a habit; add the rest (`[PublicAPI]`, `[MustUseReturnValue]`, `[MemberNotNull]`, `[DoesNotReturnIf]`, `[StringSyntax]`, …) wherever their exact condition holds.

## Asynchronous code

- Use `Task`/`Task<T>`; reserve `async void` for a framework-required event handler.
- Flow `CancellationToken` through I/O, broker, storage, and long-running work.
- Never block an async flow with `.Result`, `.Wait()`, or synchronous sleeps.

## Data contracts

- Use `record` for DTOs, message contracts, and configuration-style data.
- Use `init` properties unless a caller must mutate after construction.
- Keep a contract stable and explicit once another process or service depends on it.

## Error handling

- Validate untrusted input at a public boundary.
- Prefer a semantic exception over a generic `Exception`.
- Catch only at a boundary that can add context, clean up, or choose recovery — never a broad catch with no recovery.

## Performance

- Measure a hot path before micro-optimizing.
- Remove repeated parsing, allocation, and intermediate collections before reaching for a lower-level primitive.
- Use `Span<T>`/`ReadOnlySpan<T>` only when it materially removes copying without obscuring ownership.
- Keep expensive work out of constructors and deserialization paths; dispose owned streams and pooled resources in their owner scope.

## Review

- [ ] Identifiers carry full domain meaning; async methods end in `Async`.
- [ ] Nullability is explicit and annotations match real contracts.
- [ ] Async paths flow `CancellationToken` and never block.
- [ ] Shared data contracts are immutable `record` types.
- [ ] Exceptions are semantic and caught only where recovery or context is added.
