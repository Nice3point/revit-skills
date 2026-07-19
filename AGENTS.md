# Repository Instructions

This repository publishes independent Agent Skills plugins from `plugins/`.
Each plugin installs on its own into Claude Code or Codex.
Authoring, editing, and validating `SKILL.md` files is the main work here.
Treat every skill as a production asset, and apply the rules below whenever you create or change one.

## Repository layout

```text
plugins/<plugin>/
  plugin.json                      Claude-compatible plugin manifest
  .codex-plugin/plugin.json        Codex manifest — byte-for-byte identical to plugin.json
  skills/<skill-name>/    
    SKILL.md                       one focused workflow
    references/                    overflow docs (optional, ≤1 level deep)
    scripts/  assets/              optional bundled files
.claude-plugin/marketplace.json    Claude Code marketplace
.agents/plugins/marketplace.json   Codex marketplace
```

## Non-negotiables

- Keep `plugins/<plugin>/plugin.json` byte-for-byte identical to `plugins/<plugin>/.codex-plugin/plugin.json`.
- Keep both marketplace manifests aligned with the plugin directories.
- Marketplace versions come from `GitVersion.yml`; never hand-edit the `version` field in a manifest.
- Run `dotnet run --project build -c Release` after any change to a manifest, marketplace, or skill, and confirm it passes.
- Never add credentials, private details, or steps that need proprietary tooling.
- Never copy text from another repository; rewrite in this repo's style.

## Authoring a skill

A skill teaches an agent to perform one task well with the current best API.

### Frontmatter — exactly three keys

```yaml
---
name: <kebab-case, equals the folder name>
description: <the router — see below>
license: MIT
---
```

### Naming

Use kebab-case optimized for keyword overlap with how a developer phrases the task.
Verb-first (`configuring-…`), gerund (`writing-…`), and topic-noun (`revit-ribbon`) forms are all fine.
Keep the technology prefix (`revit-…`, `dotnet-…`) to stake a namespace.

### Description is the router

The runtime loads a skill by reading only its `description`.
Write one lead sentence of what the skill produces, then `USE FOR:` triggers, then an optional `DO NOT USE FOR:` clause.
Keep it high-level: state outcomes and intents, never method-name lists, because APIs drift and a stale list misroutes the agent.

### Granularity

Give each skill one narrow outcome.
Split a broad topic into focused skills, and add one thin end-to-end skill that references them when they form a pipeline.

### Body template (≤500 lines)

```
# Title
<one paragraph: what it produces; for a wrapper library, that it wraps the raw API for readability>
## When to use             bullets
## When not to use         only when a real collision or limitations exist; otherwise omit
## Workflow
### Step 1: …              numbered; the final step verifies the result
## Validation              - [ ] checklist a reviewer can run or observe
## Common Pitfalls         2-column table: pitfall → correct approach
```

Write load-bearing, copy-pasteable fenced code in the target language.
Add a `// BAD` → `// GOOD` contrast only where it teaches.

## House-style rules — self-check every one

1. **Teach the current best way, not API history.** State the good API positively; never document that an old API is `[Obsolete]`, legacy, or deprecated unless steering the agent away from it is a genuine pitfall.
2. **Exclude only real collisions.** A `DO NOT USE FOR` or `When not to use` entry is valid only when a competent agent would otherwise wrongly pick this skill.
    Never exclude a concern that co-applies, since a foundational skill runs alongside focused ones.
3. **Do not cap the toolset.** When the real option set is large, give the principle plus a few examples and signal it is open with `…`, rather than an exhaustive-looking list the agent treats as complete.
4. **Stay internally consistent.** Keep the frontmatter, body, validation, examples, and pitfalls in agreement.
5. **Ground every snippet in real source.** Verify each type, method, property, and argument order against the library or repo, and invent nothing.
6. **Name the dependency package** so a missing member reads as "the package is not referenced" (for example, requires `Nice3point.Revit.Extensions`).
7. **State the true scope.** Do not over-qualify with "production" when a rule applies to every case.
8. **Keep the structure.** Give each skill section headers, numbered steps for any procedure, and at least one fenced code block unless it is a pure rules or checklist skill.

## Plugin self-containment

Each plugin installs alone, so a skill must never depend on a skill in another plugin.

- Reference a sibling skill in the same plugin by its bare skill name (`use revit-ribbon`).
- Reference a capability in another plugin by its concrete API or concept, never by skill name (`use RevitContext.BeginDialogSuppressionScope`, not `use revit-context-access`).
- Keep markdown file links inside the skill directory: at most one level deep, with no `..`, absolute, or repo-rooted paths.

## Writing style

- Open with the fact the reader needs.
- Cut anything an agent can infer from the heading, signature, or surrounding text.
- Describe observable behavior and contracts, not the current implementation.
- Keep one idea per sentence, and write one sentence per line.
- Use numbered steps for procedures and checklists for requirements.
- Define a term on first use.
- State a negative only when a competent agent would otherwise make a plausible, harmful assumption.
- Wrap the frontmatter `description` by whole sentences, and put one space before an inline `//` comment.

## Structural limits (hard)

| Field / asset                        | Limit                                                                                                                           |
|--------------------------------------|---------------------------------------------------------------------------------------------------------------------------------|
| `name`                               | 1–64 chars, lowercase alphanumeric and hyphens, no leading, trailing, or doubled hyphen, equals the folder name                 |
| `description`                        | 1–1024 chars, meaningful rather than a stub                                                                                     |
| Body length                          | ≤500 lines; move overflow into `references/` behind a `**Load when:** …` trigger                                                |
| Skill size                           | ~800–2,500 tokens is the sweet spot; avoid over 5,000, which degrades performance — split instead                               |
| `references/`, `scripts/`, `assets/` | at most one directory deep, each bundled file ≤5 MB                                                                             |
| Per plugin                           | Keep the combined skill descriptions modest; the runtime skill menu truncates near 15,000 chars and silently hides later skills |

## References and links

- Use HTTPS only, never `http://`.
- Link only domains the project already trusts.
- Never pipe to a shell (`curl … | bash`), and never load a `<script>` without an `integrity` attribute.

## Validation before you finish

Per skill:

- [ ] `name` equals the folder and is valid kebab-case; `description` is a lead sentence plus `USE FOR:` with no method-name list; `license: MIT` is present.
- [ ] The skill teaches the current API only, excludes only real collisions, does not cap the toolset, and stays internally consistent.
- [ ] Every snippet is grounded in real source, the dependency package is named, and wrapper framing appears where relevant.
- [ ] The body is ≤500 lines and in the detailed token range, with overflow in `references/` behind a `Load when:` trigger.
- [ ] No cross-plugin skill-name reference remains, and every sibling reference resolves within the plugin.

Repository-wide:

- [ ] `plugin.json` equals `.codex-plugin/plugin.json`, and both marketplaces match the plugin directories.
- [ ] `dotnet run --project build -c Release` passes.
