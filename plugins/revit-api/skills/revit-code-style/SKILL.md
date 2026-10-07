---
name: revit-code-style
description: >
  Structure Autodesk Revit API code — API boundaries, document and transaction lifetime, thread affinity, and converting Revit objects to plain models.
  USE FOR: structuring Revit API code — which projects may reference Revit types, which scope opens and closes a document, how to scope a transaction, thread affinity, and converting Revit objects before they cross a service or process boundary.
  DO NOT USE FOR: the mechanics of individual model operations (querying, parameter read/write, *Utils wrappers), which have their own focused skills — apply this to the structure around them.
license: MIT
---

# Revit Code Style

Restrict Revit API types to Revit-aware projects, manage document and transaction lifetime explicitly, and convert Revit objects to plain models before they cross the boundary of a Revit-aware project.

## When to use

- Placing Revit API code and deciding which project may reference it.
- Structuring which scope opens, modifies, and closes a document and its transactions.
- Deciding what crosses a service or process boundary.

## API boundaries

- Reference the Autodesk Revit API only from a Revit-aware project.
- Exclude Revit types from routing, message contracts, serialization, and generic hosting.
- Convert Revit objects to plain, immutable models before data crosses a service or process boundary.
- Verify an unfamiliar Revit or Nice3point API against its official documentation or source.

## Lifetime and threading

- Open a document in the scope responsible for processing it, and close it and dispose generated resources in that same scope.
- Limit a transaction to one visible model change, and name it after that change.
- Treat Revit API objects as thread-affine.
- Run general I/O outside a Revit API execution context unless the API requires it.

## Reuse before writing helpers

- Prefer the `Nice3point.Revit.Extensions` fluent wrappers over raw Revit calls (`revit-element-and-parameter-access`, `revit-element-collector`, `revit-utils-extensions`).
- Prefer `Nice3point.Revit.Toolkit` context, options, and callbacks over recreating their contracts.
- Make a local extension small, deterministic, and explicit about its cost.
- Name a local extension after the collector, mutation, or file operation it performs.
- Cover a non-trivial local extension with a Revit test.

## Validation

- [ ] Only Revit-aware projects reference Revit API types.
- [ ] Documents and generated resources are closed and disposed in the scope that opened them.
- [ ] Transactions are short and named for the change.
- [ ] Revit objects are converted to plain models before crossing a boundary.

## Common Pitfalls

| Pitfall                                                | Correct approach                                                 |
|--------------------------------------------------------|------------------------------------------------------------------|
| A Revit type on a message contract or serialized model | Convert to a plain model inside the Revit-aware boundary.        |
| A transaction spanning unrelated work                  | Limit each transaction to one model change.                      |
| A local helper duplicating a Nice3point extension      | Use the existing extension; add a local one only when none fits. |
