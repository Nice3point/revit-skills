# Contributing

Thanks for helping improve Revit Agent Skills.

## Add or change a skill

1. Put each focused workflow in `plugins/<plugin>/skills/<skill-name>/SKILL.md`.
2. Use a lowercase kebab-case folder and matching `name` in the YAML frontmatter.
3. Make `description` state both the outcome and when the skill should be selected. Keep the body concise; put lengthy material in `references/` only when it is needed repeatedly.
4. Update the relevant plugin description and the root README when the public capability changes.
5. Run `dotnet run --project build -c Release` before opening a pull request.

Do not add credentials, private project details, or instructions that require unavailable proprietary tooling.

## Plugin versioning

The marketplace uses [GitVersion](https://gitversion.net/) and the repository-level [GitVersion.yml](GitVersion.yml), matching the release model used by the other repositories. `main` runs in `ContinuousDeployment` mode, so every commit between two release tags resolves to the same computed version — the generated manifest-stamp commits therefore do not change the version, and no path filtering is required.

```text
plugins/<plugin>/plugin.json
plugins/<plugin>/.codex-plugin/plugin.json
```

Do not edit the `version` property in either manifest. The scheduled [SyncPluginVersions workflow](.github/workflows/SyncPluginVersions.yml) runs the same C# `sync` pipeline locally available to contributors: it computes the authoritative version, commits the stamped manifests to `bot/plugin-version-sync`, and opens or updates its pull request. A compatible content change increments the GitVersion patch number; declare a minor or major release through `GitVersion.yml` and the release tag history.

## Pull requests

Keep a pull request focused on one plugin or a small related set of documentation changes. Explain the practical prompt or Revit task the skill supports and how you validated it.
