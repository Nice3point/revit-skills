## Agent Skills for building Autodesk Revit add-ins

[![Last Commit](https://img.shields.io/github/last-commit/Nice3point/revit-skills/main?style=for-the-badge&color=1A1A1A&labelColor=C42A2A)](https://github.com/Nice3point/revit-skills/commits/main)
[![License](https://img.shields.io/github/license/Nice3point/revit-skills?style=for-the-badge&color=1A1A1A&labelColor=C42A2A)](LICENSE)

This repository contains focused [Agent Skills](https://agentskills.io) for building Autodesk Revit add-ins. Install only the plugins that match the work at hand: the Revit API, UI, solution setup, testing, or benchmarking.

## Plugins

| Plugin                                            | Description                                                                                            |
|---------------------------------------------------|--------------------------------------------------------------------------------------------------------|
| [revit-api](plugins/revit-api/)                   | Build Revit model automation with the Revit API.                                                       |
| [revit-ui](plugins/revit-ui/)                     | Build Revit add-in interfaces and user interactions with the Revit UI API.                             |
| [revit-solution](plugins/revit-solution/)         | Build and maintain Revit add-in projects with `Nice3point.Revit.Sdk` and `Nice3point.Revit.Templates`. |
| [revit-testing](plugins/revit-testing/)           | Write automated Revit API tests with `Nice3point.TUnit.Revit`.                                         |
| [revit-benchmarking](plugins/revit-benchmarking/) | Benchmark Revit API code inside Revit with `Nice3point.BenchmarkDotNet.Revit`.                         |
| [dotnet](plugins/dotnet/)                         | Write consistent, well-documented C# for .NET projects.                                                |
| [dotnet-advanced](plugins/dotnet-advanced/)       | Design the architecture and runtime composition of modern .NET applications.                           |

## Installation

### Claude Code

1. Add the marketplace:

   ```text
   /plugin marketplace add Nice3point/revit-skills
   ```

2. Install the plugin you need:

   ```text
   /plugin install <plugin>@revit-skills
   ```

   Replace `<plugin>` with a plugin from the table above, such as `revit-api`, `revit-ui`, or `revit-testing`.

3. Restart Claude Code to load the installed plugin.

4. View the available skills:

   ```text
   /skills
   ```

5. Update an installed plugin when a new version is published:

   ```text
   /plugin update <plugin>@revit-skills
   ```

To refresh the marketplace catalogue itself:

```text
/plugin marketplace update revit-skills
```

### Codex

1. Register the repository marketplace:

   ```bash
   codex plugin marketplace add Nice3point/revit-skills
   ```

2. Open the plugin browser:

   ```text
   /plugins
   ```

3. Open **Revit Agent Skills** and install the plugins you need.

4. Refresh the marketplace when updates are published:

   ```bash
   codex plugin marketplace upgrade revit-skills
   ```

### VS Code / VS Code Insiders (Preview)

> [!IMPORTANT]
> Plugin support in VS Code is a preview feature and may change. Enable it in your settings first.

```jsonc
// settings.json
{
  "chat.plugins.enabled": true,
  "chat.plugins.marketplaces": ["Nice3point/revit-skills"]
}
```

Then type `/plugins` in Copilot Chat, or use the `@agentPlugins` filter in the Extensions view, to browse and install the plugins you need.

### Cursor

1. Open the plugin marketplace panel in Cursor.

2. Search for `revit` and install the plugins you need.

For a local checkout, copy or symlink the repository into `~/.cursor/plugins/local/revit-skills`, then run **Developer: Reload Window**.

### Manual installation

Copy a `plugins/<plugin>/skills/<skill>/` directory into your project's `.claude/skills/` for Claude Code or `.agents/skills/` for Codex.
