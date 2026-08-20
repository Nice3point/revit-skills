# Solution Filters

A solution filter is a `.slnf` file naming one solution and a subset of its projects.
An IDE opens only the named projects, and `dotnet build` restores and builds only that subset.
The solution itself is unchanged.
A filter is a view, never a second source of truth.

## Format

```json
{
  "solution": {
    "path": "Contoso.slnx",
    "projects": [
      "install/Contoso.Installer/Contoso.Installer.csproj",
      "source/Contoso.Controls/Contoso.Controls.csproj",
      "source/Contoso.Extensions/Contoso.Extensions.csproj"
    ]
  }
}
```

- `path` is relative to the filter file, and points at a `.slnx` or a `.sln`.
- Each entry in `projects` is relative to the solution.
- Build or restore a filter the same way as a solution: `dotnet build Contoso.Installer.slnf`.

## Authoring

1. **Derive the filter from the folder tree, not from a file list.**
   One filter per working set a maintainer actually opens: a deliverable and what it consumes.
   A filter that does not match any folder grouping means the tree is grouped wrong.

2. **List the full transitive closure of project references.**
   An IDE loads only what the file names.
   An unlisted dependency shows as an unresolved reference.
   Add the shared projects a maintainer edits in the same session even when nothing references them yet.

3. **Verify it opens after every solution change.**
   Filters drift when a project is renamed or moved.
   A filter that fails to open blocks the working set it names.

## When not to add one

Skip the filter when it would name most of the solution.
Every project rename requires an edit in every filter that names the project.

Skip the filter for a set that lives in a submodule with its own solution.
Open that solution instead.
The repository that defines the working set owns it.
