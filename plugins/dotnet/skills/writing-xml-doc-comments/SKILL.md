---
name: writing-xml-doc-comments
description: >
  Write or review C# XML documentation comments on public API surface.
  USE FOR: adding XML documentation elements to public types and members.
  DO NOT USE FOR: prose, markdown, README, or wiki text (use technical-writing).
license: MIT
---

# Writing XML Doc Comments

Write reference documentation held to an enterprise production standard, not tutorial or learning material.
Document the contract a caller depends on, not the mechanics a reader can see in the signature.
Every public type and member gets a `<summary>`.

## Rules

- Add a `<summary>` to every public type, member, and property.
- State the contract in `<summary>`; do not restate the name or parameter list in words.
- Add `<param>` and `<returns>` where they carry information beyond the names; state what a null or empty result means in `<summary>` or `<returns>`.
- Add `<remarks>` only for a caller-visible constraint, edge case, or ownership rule.
- Reference code symbols with `<see cref="…"/>`; renames stay linked.
- Describe observable behavior, not the current implementation.
- Describe the member as it stands, not the change that produced it; the caller reading the doc never saw the previous version.
- State facts in the present indicative; never argue why.
- Cut every purpose, result, cause, or comparison clause (`so`, `that makes`, `which makes`, `because`, `rather than`); if the clause states a fact the reader needs, make it its own sentence.

## Examples

```csharp
/// <summary>
///     Opens the file at <paramref name="path"/> and returns a reader positioned at its start.
/// </summary>
/// <param name="path">Absolute path to an existing file.</param>
/// <returns>The opened reader.</returns>
/// <remarks>The caller owns the returned reader and must dispose it.</remarks>
public StreamReader OpenReader(string path)
```

```csharp
/// <summary>
///     Returns the cached value for <paramref name="key"/>, or null when the key is absent.
/// </summary>
public CacheEntry? Find(string key)
```

## Validation

- [ ] Every public type and member has a `<summary>`.
- [ ] Summaries state the contract, not a restatement of the signature.
- [ ] Ownership rules appear in `<remarks>`; null and empty-result meaning appears in the summary or returns.
- [ ] Symbol references use `<see cref="…"/>`.
- [ ] Text states facts about the member as it stands; none narrates the change or argues why.

## Common Pitfalls

| Pitfall                                             | Correct approach                                                          |
|-----------------------------------------------------|---------------------------------------------------------------------------|
| `<summary>Gets the name.</summary>` on `GetName()`  | State what the name is and any constraint, or omit the empty restatement. |
| Documenting the implementation ("loops over items") | Describe the observable contract.                                         |
| Ownership rules left implicit                       | State them in `<remarks>`.                                                |
| Hardcoding a type name in prose                     | Use `<see cref="TypeName"/>`.                                             |
| Narrating the change ("now returns null when…")     | Describe the member as it stands.                                         |
