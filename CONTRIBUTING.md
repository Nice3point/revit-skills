# Contributing

Thanks for helping improve Revit Agent Skills.

## Add or change a skill

1. Put each focused workflow in `plugins/<plugin>/skills/<skill-name>/SKILL.md`.
2. Use a lowercase kebab-case folder and matching `name` in the YAML frontmatter.
3. Make `description` state both the outcome and when the skill should be selected. Keep the body concise; put lengthy material in `references/` only when it is needed repeatedly.
4. Update the relevant plugin description and the root README when the public capability changes.
5. Run `cd build; dotnet run` before opening a pull request.

Do not add credentials, private project details, or instructions that require unavailable proprietary tooling.

## Plugin versioning

The marketplace uses [GitVersion](https://gitversion.net/).
Do not edit the `version` property in either manifest. The [SynchronizePluginVersions workflow](.github/workflows/SynchronizePluginVersions.yml) runs the C# pipeline: it computes the version, and opens or updates its pull request. A compatible content change increments the GitVersion patch number.

```text
plugins/<plugin>/plugin.json
plugins/<plugin>/.codex-plugin/plugin.json
```

## Pull requests

Keep a pull request focused on one plugin or a small related set of documentation changes. Explain the practical prompt or Revit task the skill supports and how you validated it.
