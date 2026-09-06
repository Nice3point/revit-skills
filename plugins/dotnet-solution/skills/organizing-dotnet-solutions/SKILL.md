---
name: organizing-dotnet-solutions
description: >
    Organize a .NET solution tree: decide which solution folder each project belongs to, name the folders, and write the .slnx that maps configurations and excludes projects from a build.
    USE FOR: creating or restructuring the solution folder tree, placing a new project, naming a solution folder, mapping solution configurations onto project configurations, and scoping a working set with a solution filter.
    DO NOT USE FOR: generating new projects from a template, or arranging the types, namespaces, and service registrations inside a single project.
license: MIT
---

# Organizing .NET Solutions

The solution tree shows what the repository produces and what each project contributes to it.
Group projects by that contribution, never by the technology they are written in or the kind of file they hold.

## When to use

- Creating a solution tree, or restructuring one that has grown into a flat list.
- Placing a new project, or deciding whether an existing folder should hold it.
- Naming a solution folder, or judging whether a name still fits its contents.
- Mapping solution configurations onto project configurations in `.slnx`.

## Tiers

Every project belongs to exactly one tier, and the top level of the tree shows which.

1. **Deliverable** — something a user installs or runs.
   One folder per deliverable.
2. **Shared** — what deliverables consume.
   One folder per subject.
3. **Non-shipping** — tests, build automation, samples, and local-development sandboxes.
   Their own top-level folders.

A project the user never receives never sits inside a deliverable's folder.
A reader identifies the shipping surface from the top level alone.

## Naming a folder

Name a folder for the thing it governs, then check that the name still describes the folder once every project inside it is listed.

- Name a deliverable folder after the deliverable.
  A folder takes the name of the flagship project inside it: `/Installer/` holds `Contoso.Installer`.
- Name a shared folder after its subject.
  `/Design System/` names a subject; `/UI/` names nothing when every deliverable also has UI of its own.
- Reject a name that admits more than one kind of member.
  `Common`, `Core`, `Shared`, and `Misc` accept anything and describe nothing.
- Reject a name that describes the form of its members, such as `Services` or `Libraries`.
- If a grouping needs an invented word to be named, the grouping is wrong.

## The root of the tree

The root holds projects that need no qualifier: framework-level helpers and hosting defaults that belong to no product and no subject.
Everything else is qualified by a product, a subject, or a role, and belongs in the folder that names that qualifier.

No folder name explains a project in the root.
A root project's own name states what it is.
Rename a root project whose name only makes sense from context.

In a single-product solution the core is the product, and the root carries it.
In a multi-product solution a project serving one subject moves into that subject's folder.

## Group by shared fate

A set of projects that leaves the repository in one operation takes its own folder, whatever tier each project belongs to.
Name the folder for what the set is, and delete the folder with the set.

Shared fate outranks the tier rule only while the set has an end.
A set with no end stays in its tier.

- A folder that holds one project after a planned change does not survive that change.
  Flatten it up one level when the change lands.
- Skip the folder when the change lands within days.

## Depth

Depth equals rank.
A top-level folder names something a maintainer would say when describing what the solution produces.

Add a subfolder only when its parent already holds more than roughly six projects and the subfolder names a real division.
A subfolder holding one or two projects adds a level without adding a division.

## Writing the .slnx

The `.slnx` format is XML, and the .NET SDK reads it directly.

```xml
<Solution>
  <Configurations>
    <BuildType Name="Debug" />
    <BuildType Name="Release" />
  </Configurations>
  <Folder Name="/Solution Items/">
    <File Path="Directory.Build.props" />
    <File Path="global.json" />
  </Folder>
  <Folder Name="/Design System/">
    <Project Path="source/Contoso.Controls/Contoso.Controls.csproj" />
  </Folder>
  <Folder Name="/Installer/">
    <Project Path="install/Contoso.Installer/Contoso.Installer.csproj" />
  </Folder>
  <Folder Name="/Playground/">
    <Project Path="playground/Contoso.Playground/Contoso.Playground.csproj" />
  </Folder>
  <Folder Name="/Playground/Emulators/">
    <Project Path="playground/Contoso.Playground.Storage/Contoso.Playground.Storage.csproj" />
  </Folder>
  <Folder Name="/Tests/">
    <Project Path="tests/Contoso.Tests.Unit/Contoso.Tests.Unit.csproj" />
  </Folder>
  <Project Path="source/Contoso.Extensions/Contoso.Extensions.csproj" />
</Solution>
```

- A nested folder is a sibling element carrying the full path, not a child element.
- `<File>` puts the root files a maintainer edits into `/Solution Items/`.
- Use forward slashes in every `Path`.
- Sort entries within a folder.
- Generate the file once it passes a few hundred lines.

### Mapping configurations

A project builds in every solution configuration unless told otherwise.
Map and exclude explicitly:

```xml
<Project Path="playground/Contoso.Playground/Contoso.Playground.csproj">
  <BuildType Solution="Release.Preview|*" Project="Release" />
  <Build Solution="Release.Preview|*" Project="false" />
</Project>
```

- `<BuildType Solution="…" Project="…" />` maps one solution configuration onto the project configuration to build.
- `<Build Project="false" />` excludes the project from every configuration; adding `Solution="…"` narrows the exclusion to one.

When a solution carries configurations beyond `Debug` and `Release` — one per target platform, framework, or host version — classify every project once.

| Project role                    | Mapping                                                                              |
|---------------------------------|--------------------------------------------------------------------------------------|
| Owns the extra configurations   | Map plain `Debug`/`Release` onto one representative, and exclude them from the build |
| Shared, needed by every variant | Map each extra configuration onto `Debug`/`Release`, and build everywhere            |
| Outside the variant matrix      | Map onto `Debug`/`Release`, and exclude from every extra configuration               |

A project outside the matrix that lacks the exclusion compiles once per configuration for no result.

### Scoping a working set

[references/solution-filters.md](references/solution-filters.md) — **Load when:** a subset of a large solution should load and build on its own.

## Validation

- [ ] Every project sits in a folder, or in the root as a project with no qualifier.
- [ ] Every folder name still describes the folder once its projects are listed.
- [ ] No folder name admits more than one kind of member, and none names the form of its members.
- [ ] Tests, build automation, and sandboxes sit outside every deliverable folder.
- [ ] Every project is classified into one configuration group, and projects outside the variant matrix are excluded from its configurations.
- [ ] Paths use one separator and entries are sorted.
- [ ] `dotnet sln <solution>.slnx list` lists every expected project.

## Common Pitfalls

| Pitfall                                                                | Correct approach                                                                       |
|------------------------------------------------------------------------|----------------------------------------------------------------------------------------|
| Shared projects left loose in the root                                 | The root is for projects with no qualifier; a subject gets its own folder              |
| One folder holding both a shared library and a deliverable's own views | Name the shared folder for its subject; a deliverable's views stay with it             |
| Test or sandbox project filed inside the deliverable's folder          | Keep the non-shipping tier in its own top-level folder                                 |
| A shared-fate folder kept after its set is gone                        | Flatten the surviving folder up one level                                              |
| Sandbox or tool project with no `<Build … Project="false" />`          | Exclude it from every configuration it produces no result in                           |
| Folder named for a class of file, such as `Services`                   | Name what the folder is about                                                          |
| Mixed `\` and `/` in `Path` attributes                                 | Normalize to `/`; mixed separators come from editing the file on two operating systems |
