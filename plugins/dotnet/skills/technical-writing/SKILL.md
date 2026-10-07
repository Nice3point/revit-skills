---
name: technical-writing
description: >
    Write or review reference prose in the house register — markdown documents, wiki pages, README and CONTRIBUTING text, and configuration comments.
    USE FOR: explaining a contract, a behavior, a decision, or an API in human-readable text, and reviewing that a text says something a reader cannot already infer.
    DO NOT USE FOR: comments inside source code and documentation comments on an API, both of which follow the conventions of the language; or the body of an issue, a pull request, or a comment on one, whose shape and register the workflow standard states.
license: MIT
---

# Technical Writing

Write to an enterprise production standard in a strict technical register, never a tutorial or a marketing voice.
Open with the fact the reader needs, describe observable behavior, and cut anything a reader can already infer.
Every reference text follows this standard: a wiki page, a README, a CONTRIBUTING guide, and a comment inside a configuration file.
The standard follows the Microsoft Writing Style Guide: lead with the fact, write short paragraphs, favor the active voice and the serial comma, capitalize a heading in sentence case, spell out a Latin abbreviation in full, and name a person without gender.
The register is third person.
A stranger reads a reference page, and the page never addresses the reader as "you".

## When to use

- Writing or reviewing reference markdown in a repository.
- Reviewing a text that reads as a narration of the change instead of a statement of the result.
- Writing or reviewing a comment that annotates a declaration, a key, or a block of a configuration file.

## Rules

- Use plain technical English and the third-person present indicative in reference prose.
- Write in the active voice: the subject performs the action, and a value is never acted upon by an unnamed actor.
- Write one sentence per line, with one idea per sentence.
- Ignore the line length; a sentence occupies one line however long it runs, and no line is wrapped by hand.
  The IDE reflows the file to the limits configured for it.
- Place a comma before the conjunction in a list of three or more items.
- Spell out `for example`, `that is`, and `and so on`; never `e.g.`, `i.e.`, or `etc.`.
- Name a person, a role, or an audience without gender, and without an idiom tied to one culture.
- Use a heading that names its subject, with a colon where the heading introduces a variant or a qualifier.
  Capitalize it in sentence case, its first word and any proper noun alone, except a section a template fixes or a title matching its file's name.
- Use a list only where the reader acts on, compares, or remembers several items.
- Use a table where several items share the same set of attributes.
- State a negative only where a competent reader would otherwise make a plausible, harmful assumption.
- Link the maintained list of identifiers, endpoints, or values; never copy it.
- State no count and no enumeration the neighbouring source already lists: the rows of a table, the members of an adjoining list, the files of a directory.
  The reader reads them in the source, and a restatement becomes outdated when an item is added.
- Avoid corporate language, filler, meta-preambles, and trailing `including…` examples.
- Comment the intent, the constraint, or the invariant a file cannot state itself; add none where the file already states it.
  The name of a declaration states its meaning, and a comment is added only where a competent reader draws a wrong conclusion without one.
  A comment never restates the name, the value, or the block below it.
- Judge every sentence as final standalone text.
  The reader has the page as it stands, with no previous version, diff, or request to compare against.
- Cut every purpose, result, cause, and comparison clause: `so`, `that makes`, `which makes`, `because`, `rather than`.
  A clause that states a fact the reader needs becomes its own sentence.
- Name every operation with the term its own domain defines.
  A component requests, returns, requires, reports, or selects; a value is passed or assigned; a subscriber receives an event; a person is notified.
- Name every relation with the verb specific to its subject.
  A text states, lists, defines, or describes; a file contains or stores; a type declares or exposes; a method returns; a rule applies to its scope.

## Examples

```markdown
<!-- BAD -->
This page describes external events. We added them because the API is not reachable from a modeless window, which makes a direct call fragile, so a queue was introduced to solve the problem.

<!-- GOOD -->
An external event executes a unit of work in the Revit API context.
A caller constructs the event and raises it from any thread, and Revit invokes the handler inside the API context.
An external event opens no transaction; the handler opens its own.
```

```markdown
<!-- BAD -->
The view model asks the repository for the open document and answers the command with the result.
The binding wants a source that is not null, and the validator decides whether the entry is valid.
An event travels to every subscriber.

<!-- GOOD -->
The view model requests the open document from the repository and returns the result to the command.
The binding requires a source that is not null, and the validator reports whether the entry is valid.
Every subscriber receives the event.
```

A comment on a declaration names the role of that declaration in the system, or the invariant behind a value.

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
# The second instance serves the route while a rolling deployment replaces the first.
instances = environment == "production" ? 2 : 1
```

## Validation

- [ ] The text describes behavior, not implementation mechanics.
- [ ] Prose follows one-sentence-per-line formatting, and no line is wrapped at a column limit.
- [ ] Every sentence states a fact in the present indicative and the active voice, and none narrates the change or argues why.
- [ ] A list of three or more items takes the serial comma, and no `e.g.`, `i.e.`, or `etc.` appears.
- [ ] Every heading not fixed by a template, and not a title matching its file's name, is sentence case.
- [ ] No term names a gender or an idiom tied to one culture.
- [ ] No list of constants, endpoints, or options is copied where the authoritative source can be linked.
- [ ] No sentence states a count or an enumeration the neighbouring source already lists.
- [ ] The first sentence of a section adds information the heading does not.
- [ ] Every comment states what its file cannot, and none restates the name, the value, or the block below it.
- [ ] Every commented declaration is one a reader would otherwise misread, and the rest have no comment.
- [ ] Every operation and every relation is named by the verb its domain defines: a text states or lists, a file contains or stores, a type declares, a method returns, and a component requests or reports.

## Common Pitfalls

| Pitfall                                                        | Correct approach                                        |
|----------------------------------------------------------------|---------------------------------------------------------|
| A preamble before the point ("This section describes…")        | Lead with the fact the reader needs.                    |
| Copying a list of constants or endpoints into prose            | Link the authoritative source.                          |
| A count or an enumeration the source beside it lists           | Link the source.                                        |
| Restating the heading in the first sentence                    | Add new information.                                    |
| Documenting how the code works today                           | Document the observable contract.                       |
| Narrating the edit ("renamed X to Y because…")                 | State what the code now is.                             |
| A rationale clause ("… so …", "… that makes …", "rather than") | State each fact in its own present-indicative sentence. |
| "The validator decides whether the entry is valid."            | "The validator reports whether the entry is valid."     |
| A paragraph promising work still to come                       | Add a `// TODO:` in the code at the place it belongs.   |
| A paragraph hard-wrapped at 80 or 120 characters               | One sentence, one line, whatever its length.            |
| A comment naming the key below it                              | State the constraint on the key.                        |
| A comment paraphrasing the block it opens                      | Drop it; the block states itself.                       |
| A comment pointing at its own file                             | State the role of the declaration in the system.        |
| A comment above every declaration of a file                    | Comment the one declaration a reader misreads.          |
| A name that needs a comment to be understood                   | Rename the declaration.                                 |
| "The setting is read by the host at startup."                  | "The host reads the setting at startup."                |
| `e.g.`, `i.e.`, or `etc.` in prose                             | Spell out `for example`, `that is`, `and so on`.        |
| `## Common Configuration Options`                              | `## Common configuration options`.                      |
| "The section carries three facts."                             | "The section lists three facts."                        |
| "The fact lives in the readme."                                | "The readme states the fact."                           |
| "The rule reaches every section."                              | "The rule applies to every section."                    |
| "The table holds the supported versions."                      | "The table lists the supported versions."               |
| "The configuration file keeps the connection string."          | "The configuration file stores the connection string."  |
| "The response carries the status code."                        | "The response contains the status code."                |
