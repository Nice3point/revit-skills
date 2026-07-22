---
name: revit-code-style
description: >
  Structure Autodesk Revit API code — API boundaries, document and transaction ownership, thread affinity, and converting Revit objects to plain models.
  USE FOR: structuring Revit API code — where Revit types may live, who opens and closes a document, how to scope a transaction, thread affinity, and converting Revit objects before they cross a service or process boundary.
  DO NOT USE FOR: the mechanics of individual model operations (querying, parameter read/write, *Utils wrappers), which have their own focused skills — apply this to the structure around them.
license: MIT
---

# Revit Code Style

Keep Revit API types contained, own document and transaction lifetime explicitly, and convert Revit objects to plain models before they leave a Revit-aware boundary.

## When to use

- Placing Revit API code and deciding which project may reference it.
- Structuring who opens, mutates, and closes a document and its transactions.
- Deciding what crosses a service or process boundary.

## API boundaries

- Keep Autodesk Revit API references inside a Revit-aware project; keep routing, message contracts, serialization, and generic hosting free of Revit types.
- Convert Revit objects to plain, immutable models before data crosses a service or process boundary.
- Verify an unfamiliar Revit or Nice3point API against its official documentation or source.

## Ownership and threading

- Open a document in the scope that owns processing it; close it and dispose generated resources in the matching owner scope.
- Keep transactions short and named after the visible model change.
- Treat Revit API objects as thread-affine; keep general I/O outside a Revit API execution context unless the API requires it.

## Reuse before writing helpers

- Prefer the `Nice3point.Revit.Extensions` fluent wrappers over raw Revit calls (`revit-element-and-parameter-access`, `revit-element-collector`, `revit-utils-extensions`).
- Prefer `Nice3point.Revit.Toolkit` context, options, and callbacks over recreating their contracts.
- Keep a local extension small, deterministic, and explicit about cost; do not hide a collector, mutation, or file operation behind an innocuous name. Cover a non-trivial local extension with a Revit test.

## Validation

- [ ] Revit API types stay inside Revit-aware projects.
- [ ] Documents and generated resources are closed and disposed in their owner scope.
- [ ] Transactions are short and named for the change.
- [ ] Revit objects are converted to plain models before crossing a boundary.

## Common Pitfalls

| Pitfall                                                | Correct approach                                                 |
|--------------------------------------------------------|------------------------------------------------------------------|
| A Revit type on a message contract or serialized model | Convert to a plain model inside the Revit-aware boundary.        |
| A transaction spanning unrelated work                  | Keep transactions short and single-purpose.                      |
| A local helper duplicating a Nice3point extension      | Use the existing extension; add a local one only when none fits. |
