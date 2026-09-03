---
name: technical-writing
description: >
  Write or review technical prose — markdown documentation, wiki pages, README or config comments.
  USE FOR: explaining a contract, behavior, decision, or API in human-readable text, and reviewing that prose says something a reader cannot already infer.
  DO NOT USE FOR: C# code comments (use csharp-style), or C# XML documentation comments (use writing-xml-doc-comments).
license: MIT
---

# Technical Writing

Write to an enterprise production standard in a strict technical register, never a tutorial or a marketing voice.
Open with the fact the reader needs, describe observable behavior, and cut anything a reader can already infer.

## When to use

- Writing or reviewing any markdown in a repository.
- Writing the body of an issue, a pull request, or a comment on one.
- Reviewing a text that reads as a narration of the change instead of a statement of the result.
- Writing or reviewing a comment that annotates a declaration, a key, or a block of a configuration file.

## Rules

- Use plain technical English and the third-person present indicative in reference prose.
- Write one sentence per line, and keep one idea per sentence.
- Ignore the line length; a sentence occupies one line however long it runs, and no line is wrapped by hand.
  The IDE reflows the file to the limits configured for it.
- Use a heading that names its subject, with a colon where the heading introduces a variant or a qualifier.
- Use a list only where the reader acts on, compares, or remembers several items.
- Use a table where several items share the same set of attributes.
- State a negative only where a competent reader would otherwise make a plausible, harmful assumption.
- Link the maintained list of identifiers, endpoints, or values; never copy it.
- State no count and no enumeration the neighbouring source already carries: the rows of a table, the members of a list below, the files a directory holds.
  The reader takes them from the source, and a restatement falls out of date on the change that adds one.
- Avoid corporate language, filler, meta-preambles, and trailing `including…` examples.
- Comment the intent, the constraint, or the invariant a file cannot state itself; add none where the file already states it.
  A declaration carries its own meaning through its name, and a comment stands only where a competent reader draws a wrong conclusion without one.
  A comment never restates the name, the value, or the block it stands above.
- Judge every sentence as final standalone text.
  The reader has the page as it stands, with no previous version, diff, or request to compare against.
- Cut every purpose, result, cause, and comparison clause: `so`, `that makes`, `which makes`, `because`, `rather than`.
  A clause carrying a fact the reader needs becomes its own sentence.

## Examples

```markdown
<!-- BAD -->
This page describes external events. We added them because the API is not reachable from a modeless window, which makes a direct call fragile, so a queue was introduced to solve the problem.

<!-- GOOD -->
An external event carries a unit of work into the Revit API context.
A caller constructs the event and raises it from any thread, and Revit invokes the handler inside the API context.
An external event opens no transaction; the handler opens its own.
```

A comment on a declaration names the role that declaration holds in the whole system, or the invariant behind a value.

```text
# BAD
# The catalogue viewer, a second process of the frontend group on a port of its own, declared in this same file.
# The internal gateway alone carries this route, and every public gateway resolves the frontend on its plain port, so no public host reaches it.
component "catalogue" {

# GOOD
# The gateway of the public applications and of the internal-only endpoints.
component "gateway" {

# GOOD
# The cache every service of the environment shares.
component "cache" {
```

```text
# BAD
# The instance count, two in production and one everywhere else.
instances = environment == "production" ? 2 : 1

# GOOD
# The second instance holds the route open while a rolling replacement takes the first.
instances = environment == "production" ? 2 : 1
```

## Review

- [ ] The text describes behavior, not implementation mechanics.
- [ ] Prose follows one-sentence-per-line formatting, and no line is wrapped at a column limit.
- [ ] Every sentence states a fact in the present indicative, and none narrates the change or argues why.
- [ ] No list of constants, endpoints, or options is copied where the authoritative source can be linked.
- [ ] No sentence states a count or an enumeration the neighbouring source carries.
- [ ] The first sentence of a section carries information the heading does not.
- [ ] Every comment states what its file cannot, and none restates the name, the value, or the block below it.
- [ ] Every commented declaration is one a reader would otherwise misread, and the rest carry no comment.

## Common Pitfalls

| Pitfall                                                        | Correct approach                                        |
|----------------------------------------------------------------|---------------------------------------------------------|
| A preamble before the point ("This section describes…")        | Lead with the fact the reader needs.                    |
| Copying a list of constants or endpoints into prose            | Link the authoritative source.                          |
| A count or an enumeration the source beside it carries         | Point at the source; the reader reads it there.         |
| Restating the heading in the first sentence                    | Add new information.                                    |
| Documenting how the code works today                           | Document the observable contract.                       |
| Narrating the edit ("renamed X to Y because…")                 | State what the code now is.                             |
| A rationale clause ("… so …", "… that makes …", "rather than") | State each fact in its own present-indicative sentence. |
| A paragraph promising work still to come                       | Leave a `// TODO:` in the code at the place it belongs. |
| A paragraph hard-wrapped at 80 or 120 characters               | One sentence, one line, whatever its length.            |
| A comment naming the key it stands above                       | State the constraint the key carries.                   |
| A comment paraphrasing the block it opens                      | Drop it; the block states itself.                       |
| A comment pointing at the file it lives in                     | State the role the declaration holds in the system.     |
| A comment above every declaration of a file                    | Comment the one declaration a reader misreads.          |
| A name that needs a comment to be understood                   | Rename the declaration.                                 |
