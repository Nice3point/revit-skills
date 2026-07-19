---
name: technical-writing
description: >
  Write or review technical prose — markdown documentation, README and wiki pages, and code comments.
  USE FOR: explaining a contract, behavior, decision, or API in human-readable text, and reviewing that prose says something a reader cannot already infer.
  DO NOT USE FOR: C# XML documentation comments (use writing-xml-doc-comments).
license: MIT
---

# Technical Writing

Open with the fact the reader needs, describe observable behavior, and cut anything a reader can already infer.

## Write

1. Identify the reader and the decision or behavior the text must make clear.
2. Open with the fact the reader needs.
3. Describe the observable contract rather than its current implementation.
4. Link the authoritative local source when a value, schema, or API may change.
5. Remove every statement a reader can infer from the code, heading, or surrounding text.

## Apply

- Use plain technical English and third-person present indicative in reference prose.
- Write one sentence per line; keep one idea per sentence.
- Use headings that name the subject; use a colon in a heading that introduces a variant or qualifier.
- Use a list only when the reader must act on, compare, or remember several items.
- State a negative only when a competent reader would otherwise make a plausible, harmful assumption.
- Link instead of copying a maintained list of identifiers or values.
- Avoid corporate language, filler, meta-preambles, and trailing "including…" examples.

## Review

- [ ] The first sentence states the substantive fact.
- [ ] The text describes behavior, not implementation mechanics.
- [ ] Each sentence adds information a reader cannot infer.
- [ ] The authoritative source stays linked where a contract is restated.
- [ ] Prose follows one-sentence-per-line formatting.

## Common Pitfalls

| Pitfall                                                 | Correct approach                     |
|---------------------------------------------------------|--------------------------------------|
| A preamble before the point ("This section describes…") | Lead with the fact the reader needs. |
| Copying a list of constants or endpoints into prose     | Link the authoritative source.       |
| Restating the heading in the first sentence             | Add new information instead.         |
| Documenting how the code works today                    | Document the observable contract.    |
