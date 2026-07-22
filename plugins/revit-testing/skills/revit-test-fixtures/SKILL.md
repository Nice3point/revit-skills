---
name: revit-test-fixtures
description: >
  Supply Autodesk Revit API tests with the documents, services, and data cases they run against, and pick the right data source for each situation.
  USE FOR: seeding a fresh in-memory model per test, opening installed sample .rvt/.rfa files as fixtures, choosing between [MethodDataSource] and [InstanceMethodDataSource], running one test body across several file kinds, injecting services under test through a dependency-injection data source, and test skipping.
  DO NOT USE FOR: writing the test itself or the Revit-thread executor model (use revit-testing), or measuring performance (that is a benchmark, not a fixture).
license: MIT
---

# Revit Test Fixtures

A fixture is everything a test runs against: the document, the services it calls, and the cases it repeats over.
Choosing the wrong one is the usual cause of tests that pass alone but fail together, or that silently test nothing.
Match the situation to one reference below and open only that one.

Two invariants hold across every variant:

1. **Create and close documents on the Revit thread.** Any hook that opens, seeds, or closes a document carries `[HookExecutor<RevitThreadExecutor>]`, and every opened or created document is closed in teardown.
2. **Discovery runs before Revit exists.** TUnit evaluates every data source, constructs the test class, and resolves every injected service during discovery, off the Revit thread — they yield only primitives (numbers, strings, file paths) and never call the Revit API at construction. The test body turns those primitives into Revit objects on the Revit thread.

```csharp
public static string[] DocumentPaths => Directory.EnumerateFiles(directory, "*.rvt").ToArray(); // primitives, off-thread

[After(Test)]
[HookExecutor<RevitThreadExecutor>] // teardown touches Revit; it runs on the Revit thread
public void CloseDocument()
{
    _document?.Close(false);
}
```

## When to use

- A test needs a document, an injected service, or a repeated set of cases.
- Tests share state and interfere with each other.

## Choose the fixture

Match the required state to one reference and open only that reference.

- [references/seeded-document.md](references/seeded-document.md) — **Load when:** you need a fresh, exact, mutable model that the test authors in code.
- [references/parameterized-fixtures.md](references/parameterized-fixtures.md) — **Load when:** a fixed set of `.rvt`/`.rfa` files, or one test body must run for several sample directories, through `[InstanceMethodDataSource]` and `[InheritsTests]`.
- [references/dependency-injection.md](references/dependency-injection.md) — **Load when:** the test class receives its service under test from a DI container.
- [references/skipping.md](references/skipping.md) — **Load when:** a sample folder or Revit localization may be absent and the affected test must skip cleanly.

## Selecting a data source

`[Arguments]`, `[MethodDataSource]`, and `[InstanceMethodDataSource]` are basic TUnit data-driven-test features.
This skill only selects them for Revit fixtures and adds the Revit-thread boundary.
For other TUnit approaches, including class, matrix, combined, and custom data sources, read TUnit's [Method Data Sources source](https://raw.githubusercontent.com/thomhurst/TUnit/main/docs/docs/writing-tests/method-data-source.md).

- **`[Arguments]`** — a small fixed set of inline primitive cases.
- **`[MethodDataSource(nameof(Member))]`** — a **static** member whose value is fixed at discovery: a computed set of numbers or a static list of sample paths.
- **`[InstanceMethodDataSource(nameof(Member))]`** — an **instance** member whose value depends on constructor state: a base class parameterized by extension or directory, or a set built from injected configuration. See `parameterized-fixtures`.
- **A dependency-injection data-source attribute** — a custom TUnit data source that fills test-class constructor parameters from a DI container. See `dependency-injection`.

## Validation

- [ ] Each test gets isolated state; nothing leaks between tests.
- [ ] Every opened or created document is closed in `[After(Test)]`, and fixture hooks that touch Revit use `[HookExecutor<RevitThreadExecutor>]`.
- [ ] Sample files are opened from a private copy, not in place.
- [ ] A missing sample set or localization skips the affected test; it does not fail it.
- [ ] The data-source attribute matches the member: `[MethodDataSource]` for static, `[InstanceMethodDataSource]` for constructor-dependent.

## Common Pitfalls

| Pitfall                                                | Correct approach                                                                       |
|--------------------------------------------------------|----------------------------------------------------------------------------------------|
| Tests passing alone but failing together               | Give each test its own document or state; do not share mutable fixtures.               |
| A data source that returns a Revit object              | Return primitives or paths; build Revit objects in the test body on the Revit thread.  |
| `[MethodDataSource]` on a constructor-dependent member | Use `[InstanceMethodDataSource]`; the instance (and its configuration) exists first. |
| Opening the sample file in place                       | Copy it to a temporary path and open the copy.                                         |
| Failing when the samples folder is missing             | Return an empty data set (its tests are skipped) or call `Skip.Test(...)`.             |
