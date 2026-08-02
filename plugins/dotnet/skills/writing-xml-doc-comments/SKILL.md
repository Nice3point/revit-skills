---
name: writing-xml-doc-comments
description: >
  Write or review C# XML documentation comments on public API surface.
  USE FOR: adding or reviewing XML documentation on public types, members, parameters, type parameters, and return values.
  DO NOT USE FOR: prose, markdown, README, or wiki text (use technical-writing), or ordinary code comments (use csharp-style).
license: MIT
---

# Writing XML Doc Comments

Write reference documentation held to an enterprise production standard, not tutorial or learning material.
Document the contract a caller depends on, not the mechanics a reader can see in the signature.
The conventions match the .NET libraries.

## When to use

- Adding a public or protected type, member, or contract to a library or a shared assembly.
- Reviewing XML documentation for coverage, tag order, opening phrasing, and contract accuracy.
- Closing documentation gaps reported by CS1591 or CS1573.

## Rules

- Give every public and protected type, member, parameter, type parameter, and return value its tag.
- Put the text on its own line between the opening and closing tag, indented four spaces past the `///` marker; keep the one-line form for a single short clause.
- Indent a nested tag one level further, and leave a `<code>` block at the indentation its rendered output needs.
- Write one sentence per tag and end it with a period; a second sentence belongs in `<remarks>`.
- State the contract; never restate the name or the parameter list in words.
- Describe observable behavior, not the current implementation.
- Describe the member as it stands, not the change that produced it; the caller reading the doc never saw the previous version.
- State facts in the present indicative; never argue why.
- Cut every purpose, result, cause, or comparison clause (`so`, `that makes`, `which makes`, `because`, `rather than`); if the clause states a fact the reader needs, make it its own sentence.

## Tag order

Write the tags in this order.

1. `<summary>`
2. `<value>` — a property whose default or unit the caller needs
3. `<typeparam>` — one per type parameter, in declaration order
4. `<param>` — one per parameter, in declaration order
5. `<returns>`
6. `<exception>` — one per contract throw
7. `<remarks>`
8. `<example>`

## Summary opening by member kind

The skeleton is identical for every declaration; the opening phrase is what changes.

| Declaration                        | Summary opens with                                             | Example                                                                                |
|------------------------------------|----------------------------------------------------------------|----------------------------------------------------------------------------------------|
| Class, struct, record              | `Represents …`, `Provides …`, `Defines …`                      | `Represents the host portion of a URI.`                                                |
| Static class holding extensions    | `Extension methods for …`, `Provides extension methods for …`  | `Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add endpoints.` |
| Interface                          | `Defines a contract that …`, `Provides an interface for …`     | `Defines a contract that represents the result of an HTTP endpoint.`                   |
| Attribute                          | `Specifies …`                                                  | `Specifies a collection of tags in <see cref="Endpoint.Metadata"/>.`                   |
| Exception type                     | `Represents … error`                                           | `Represents an HTTP request error.`                                                    |
| Delegate                           | `A function that …`, `A delegate that …`                       | `A function that can process an HTTP request.`                                         |
| Enum                               | `Determines …`, `Indicates whether …`                          | `Determines how cookie security properties are set.`                                   |
| Enum member                        | the effect of selecting the value                              | `Opts out of compression over HTTPS.`                                                  |
| Constant, static readonly field    | the meaning of the value                                       | `HTTP status code 201.`                                                                |
| Constructor                        | `Initializes a new instance of the <see cref="T"/> class.`     | —                                                                                      |
| Read-only property                 | `Gets …`                                                       | `Gets the HTTP status code for this exception.`                                        |
| Read-write property                | `Gets or sets …`                                               | `Gets or sets the prefix used to identify the current object.`                         |
| Boolean property                   | `Gets a value indicating whether …`                            | —                                                                                      |
| Event                              | `An event that fires when …`, `An event that is raised when …` | `An event that is raised when a field value changes.`                                  |
| Factory method                     | `Creates a <see cref="T"/> …`                                  | `Creates a <see cref="ChallengeHttpResult"/> for the response.`                        |
| Registration method                | `Adds … to the specified <see cref="IServiceCollection"/>.`    | —                                                                                      |
| Override, interface implementation | `<inheritdoc/>`                                                | —                                                                                      |
| Explicit interface implementation  | no doc comment                                                 | —                                                                                      |

A delegate carries its `<param>` and `<returns>` on the type declaration.
An `extension` block carries a `<param>` for its receiver, placed on the block; each member inside the block documents only its own parameters.

## Parameters, returns, and exceptions

- Write `<param>` and `<returns>` as noun phrases opening with `The`, `A`, or `An`, and end them with a period.
- An async method returns `A task that represents the asynchronous <operation> operation.`; name the produced value when the task carries one.
- A builder or registration method returns `The <see cref="T"/> for chaining.` or `A <see cref="T"/> that can be used to further customize the …`.
- A `Try` method documents the `out` parameter as `When this method returns, contains …`, and returns `<see langword="true"/> if …; otherwise, <see langword="false"/>.`.
- State what a null or empty result means in `<summary>` or `<returns>`.
- Reserve `<exception>` for a throw that is part of the contract; an argument guard at entry gets none.
- Never leave a tag empty to silence CS1573; describe the parameter.

## Inherited documentation

- Put `<inheritdoc/>` on an override and on an implicit interface implementation, including `ToString`, `Dispose`, and a property that satisfies an interface.
- Add `<remarks>` under `<inheritdoc/>` when the implementation adds a caller-visible constraint the base contract does not state.
- Use `<inheritdoc cref="…"/>` when the source is not the immediate base member.
- Leave an explicit interface implementation undocumented; the interface holds the documentation.

## Cross-references and inline markup

- Reference every type or member named in text with `<see cref="…"/>`; renames stay linked.
- Reference the current member's parameters with `<paramref name="…"/>` and `<typeparamref name="…"/>`.
- Write keywords as `<see langword="true"/>`, `<see langword="false"/>`, `<see langword="null"/>`.
- Use `<c>` for a literal value or fragment that names no symbol.
- Link external documentation with `<see href="https://…">Title</see>`; HTTPS only.
- Use `<list type="bullet">` with `<item><description>…</description></item>` when the behavior branches on argument combinations.
- Add a prose `<example>` on a parsing or accessor member to show what a concrete input yields.
- Add `<example>` with `<code>` on an attribute or entry-point API whose call shape is not obvious from the signature; ordinary members get none.
- Use `<para>` only to split a long `<remarks>`.

## What belongs in remarks

A default value, a threading or reentrancy rule, an ownership or disposal rule, a platform limit, an interaction with another member, or the behavior on an edge input.

## Examples

```csharp
/// <summary>
///     Represents a session opened against a document store.
/// </summary>
public sealed class DocumentSession : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DocumentSession"/> class.
    /// </summary>
    /// <param name="store">The store the session reads from.</param>
    public DocumentSession(IDocumentStore store)
    {
        Store = store;
    }

    /// <summary>
    ///     Gets the store the session reads from.
    /// </summary>
    public IDocumentStore Store { get; }

    /// <summary>
    ///     Gets or sets the timeout applied to every read.
    /// </summary>
    /// <value>Defaults to 30 seconds.</value>
    public TimeSpan ReadTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Gets a value indicating whether the session holds an open transaction.
    /// </summary>
    public bool IsTransactional { get; }

    /// <summary>
    ///     An event that is raised when the session loads a document.
    /// </summary>
    public event EventHandler<DocumentLoadedEventArgs>? DocumentLoaded;

    /// <summary>
    ///     Loads the document stored under the specified identifier.
    /// </summary>
    /// <param name="documentId">The identifier of the document to load.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous load operation.
    ///     The task result contains the loaded document, or <see langword="null"/> when the identifier is unknown.
    /// </returns>
    /// <exception cref="ObjectDisposedException">The session is closed.</exception>
    /// <remarks>The caller owns the returned document and disposes it.</remarks>
    public async Task<Document?> LoadAsync(string documentId, CancellationToken cancellationToken = default)
    {
        ...
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ...
    }
}
```

```csharp
/// <summary>
///     Defines a contract that resolves a document by its identifier.
/// </summary>
public interface IDocumentResolver
{
    /// <summary>
    ///     Resolves the document stored under the specified identifier.
    /// </summary>
    /// <param name="documentId">The identifier of the document to resolve.</param>
    /// <param name="document">When this method returns, contains the resolved document if the identifier is known.</param>
    /// <returns><see langword="true"/> if the document was resolved; otherwise, <see langword="false"/>.</returns>
    bool TryResolve(string documentId, [NotNullWhen(true)] out Document? document);
}

/// <summary>
///     Resolves a document from the local file system cache.
/// </summary>
public sealed class CachedDocumentResolver : IDocumentResolver
{
    /// <inheritdoc/>
    public bool TryResolve(string documentId, [NotNullWhen(true)] out Document? document)
    {
        ...
    }
}
```

```csharp
/// <summary>
///     Determines how a session resolves a document that is absent from the cache.
/// </summary>
public enum CacheMissBehavior
{
    /// <summary>
    ///     Reads the document from the store and adds it to the cache.
    /// </summary>
    Fetch,

    /// <summary>
    ///     Returns no document and leaves the cache unchanged.
    /// </summary>
    Skip
}

/// <summary>
///     A function that transforms a document before it reaches the caller.
/// </summary>
/// <param name="document">The document to transform.</param>
/// <returns>The transformed document.</returns>
public delegate Document DocumentTransform(Document document);
```

```csharp
/// <summary>
///     Provides extension methods for <see cref="IServiceCollection"/> to register document storage.
/// </summary>
public static class DocumentStorageServiceCollectionExtensions
{
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds the document storage services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="configure">An optional action to configure the <see cref="DocumentStorageOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
        public IServiceCollection AddDocumentStorage(Action<DocumentStorageOptions>? configure = null)
        {
            ...
        }
    }
}
```

## Validation

- [ ] Every public and protected type, member, parameter, type parameter, and return value carries its tag; the build reports no CS1591 or CS1573.
- [ ] Tags appear in the order summary, value, typeparam, param, returns, exception, remarks, example.
- [ ] Multi-line tag text sits four spaces past the `///` marker, and a nested tag sits one level further.
- [ ] The summary opening matches the member kind, and a property opens with `Gets`, `Gets or sets`, or `Gets a value indicating whether`.
- [ ] Overrides and implicit interface implementations use `<inheritdoc/>`; explicit interface implementations carry no doc comment.
- [ ] Every type and member named in text is a `<see cref="…"/>`, and `true`, `false`, and `null` are `<see langword="…"/>`.
- [ ] `<exception>` lists only contract throws; argument guards at entry carry none.
- [ ] Null and empty-result meaning appears in the summary or returns; ownership, defaults, and threading rules appear in remarks or value.
- [ ] Text states facts about the member as it stands; none narrates the change or argues why.

## Common Pitfalls

| Pitfall                                                       | Correct approach                                           |
|---------------------------------------------------------------|------------------------------------------------------------|
| `<summary>Gets the name.</summary>` on `GetName()`            | State what the name is and any constraint.                 |
| A property summary without `Gets` or `Gets or sets`           | Open with the accessor verb the member exposes.            |
| Re-summarizing an override or interface implementation        | Use `<inheritdoc/>`.                                       |
| Documenting an explicit interface implementation              | Leave it undocumented; the interface carries the contract. |
| An empty `<param></param>` added to silence CS1573            | Describe the parameter.                                    |
| `<exception cref="ArgumentNullException">` for an entry guard | Document only a throw that is part of the contract.        |
| `<remarks>` placed before `<param>` on a method               | Put `<remarks>` after `<returns>`.                         |
| Multi-line tag text left flush against `///`                  | Indent it four spaces past the marker.                     |
| Documenting the implementation ("loops over items")           | Describe the observable contract.                          |
| Hardcoding a type name in prose                               | Use `<see cref="TypeName"/>`.                              |
| Writing `true`, `false`, or `null` as plain text              | Use `<see langword="true"/>`.                              |
| Ownership, default value, or unit left implicit               | State it in `<remarks>` or `<value>`.                      |
| Narrating the change ("now returns null when…")               | Describe the member as it stands.                          |
