---
name: technical-writing
description: >
  Write or review technical prose — markdown documentation, wiki pages, README or config comments.
  USE FOR: explaining a contract, behavior, decision, or API in human-readable text, and reviewing that prose says something a reader cannot already infer.
  DO NOT USE FOR: C# code comments (use csharp-style), or C# XML documentation comments (use writing-xml-doc-comments).
license: MIT
---

# Technical Writing

Write to an enterprise production standard in a strict technical register, not a tutorial or marketing voice.
Open with the fact the reader needs, describe observable behavior, and cut anything a reader can already infer.

## Rules

- Use plain technical English and third-person present indicative in reference prose.
- Write one sentence per line; keep one idea per sentence.
- Use headings that name the subject; use a colon in a heading that introduces a variant or qualifier.
- Use a list only when the reader must act on, compare, or remember several items.
- State a negative only when a competent reader would otherwise make a plausible, harmful assumption.
- Link the maintained list of identifiers or values; do not copy it.
- Avoid corporate language, filler, meta-preambles, and trailing "including…" examples.
- Judge every sentence as final standalone text: the reader has only the page as it stands, with no previous version, diff, or request to compare against; drop any sentence that merely narrates the change.
- State facts in the present indicative; never argue why.
- Cut every purpose, result, cause, or comparison clause (`so`, `that makes`, `which makes`, `because`, `rather than`); if the clause states a fact the reader needs, make it its own sentence.

## Review

- [ ] The text describes behavior, not implementation mechanics.
- [ ] Prose follows one-sentence-per-line formatting.
- [ ] Every sentence states a fact in the present indicative; none narrates the change or argues why.

## Common Pitfalls

| Pitfall                                                        | Correct approach                                        |
|----------------------------------------------------------------|---------------------------------------------------------|
| A preamble before the point ("This section describes…")        | Lead with the fact the reader needs.                    |
| Copying a list of constants or endpoints into prose            | Link the authoritative source.                          |
| Restating the heading in the first sentence                    | Add new information.                                    |
| Documenting how the code works today                           | Document the observable contract.                       |
| Narrating the edit ("renamed X to Y because…")                 | State what the code now is.                             |
| A rationale clause ("… so …", "… that makes …", "rather than") | State each fact in its own present-indicative sentence. |
