using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Validates the plugin manifests, marketplaces, and skill frontmatter shipped by this repository.
/// </summary>
public sealed partial class ValidateMarketplaceModule : SyncModule
{
    private static readonly Regex StrictSemVer = SemanticVersionRegex();

    protected override void ExecuteModule(IModuleContext context, CancellationToken cancellationToken)
    {
        var root = context.Git().RootDirectory.Path;
        var pluginRoot = Path.Combine(root, "plugins");

        var errors = new List<string>();
        var plugins = Directory.GetDirectories(pluginRoot)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        foreach (var plugin in plugins)
        {
            var pluginDirectory = Path.Combine(pluginRoot, plugin);
            ValidateManifest(root, plugin, pluginDirectory, errors);
            ValidateSkills(plugin, pluginDirectory, errors);
        }

        ValidateMarketplace(root, ".agents/plugins/marketplace.json", plugins, true, errors);
        ValidateMarketplace(root, ".claude-plugin/marketplace.json", plugins, false, errors);

        errors.ShouldBeEmpty($"Marketplace validation failed:{Environment.NewLine}- {string.Join(Environment.NewLine + "- ", errors)}");
    }

    private static void ValidateManifest(string root, string plugin, string directory, ICollection<string> errors)
    {
        var manifestPath = Path.Combine(directory, "plugin.json");
        var codexManifestPath = Path.Combine(directory, ".codex-plugin", "plugin.json");
        var manifestText = ReadFile(root, manifestPath, errors);
        if (manifestText is null) return;

        var codexManifestText = ReadFile(root, codexManifestPath, errors);
        if (codexManifestText is null) return;

        if (!string.Equals(manifestText, codexManifestText, StringComparison.Ordinal))
        {
            errors.Add($"plugins/{plugin}: plugin.json and .codex-plugin/plugin.json must be identical");
        }

        var manifest = ParseObject(root, manifestPath, errors);
        if (manifest is null) return;

        if (!string.Equals(GetString(manifest, "name"), plugin, StringComparison.Ordinal))
        {
            errors.Add($"plugins/{plugin}/plugin.json: name must equal {plugin}");
        }

        var version = GetString(manifest, "version");
        if (version is null || !StrictSemVer.IsMatch(version))
        {
            errors.Add($"plugins/{plugin}/plugin.json: version must be strict SemVer");
        }

        if (!string.Equals(GetString(manifest, "skills"), "./skills/", StringComparison.Ordinal))
        {
            errors.Add($"plugins/{plugin}/plugin.json: skills must be ./skills/");
        }

        if (GetObject(manifest, "author") is null || string.IsNullOrWhiteSpace(GetString(GetObject(manifest, "author")!, "name")))
        {
            errors.Add($"plugins/{plugin}/plugin.json: author.name is required");
        }

        var interfaceMetadata = GetObject(manifest, "interface");
        if (interfaceMetadata is null)
        {
            errors.Add($"plugins/{plugin}/plugin.json: interface metadata is required");
            return;
        }

        foreach (var field in new[] { "displayName", "shortDescription", "longDescription", "developerName", "category" })
        {
            if (string.IsNullOrWhiteSpace(GetString(interfaceMetadata, field)))
            {
                errors.Add($"plugins/{plugin}/plugin.json: interface.{field} is required");
            }
        }

        if (interfaceMetadata["defaultPrompt"] is not JsonArray { Count: > 0 })
        {
            errors.Add($"plugins/{plugin}/plugin.json: interface.defaultPrompt is required");
        }

        if (interfaceMetadata["capabilities"] is not JsonArray { Count: > 0 })
        {
            errors.Add($"plugins/{plugin}/plugin.json: interface.capabilities is required");
        }
    }

    private static void ValidateSkills(string plugin, string directory, List<string> errors)
    {
        var skillsDirectory = Path.Combine(directory, "skills");
        foreach (var skillDirectory in Directory.GetDirectories(skillsDirectory))
        {
            var skillName = Path.GetFileName(skillDirectory);
            var skillPath = Path.Combine(skillDirectory, "SKILL.md");
            if (!File.Exists(skillPath)) continue;

            var lines = File.ReadAllLines(skillPath);
            if (lines.Length == 0 || lines[0] != "---")
            {
                errors.Add($"plugins/{plugin}/skills/{skillName}/SKILL.md: expected YAML frontmatter");
                continue;
            }

            var closingIndex = Array.IndexOf(lines, "---", 1);
            if (closingIndex < 0)
            {
                errors.Add($"plugins/{plugin}/skills/{skillName}/SKILL.md: expected a closing YAML frontmatter marker");
                continue;
            }

            var frontmatter = lines[1..closingIndex];
            var name = ReadFrontmatterValue(frontmatter, "name");
            var description = ReadFrontmatterValue(frontmatter, "description");
            if (!string.Equals(name, skillName, StringComparison.Ordinal))
            {
                errors.Add($"plugins/{plugin}/skills/{skillName}/SKILL.md: name must equal {skillName}");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                errors.Add($"plugins/{plugin}/skills/{skillName}/SKILL.md: description is required");
            }
        }
    }

    private static void ValidateMarketplace(string root, string relativePath, IReadOnlyCollection<string> plugins, bool codex, ICollection<string> errors)
    {
        var marketplacePath = Path.Combine(root, relativePath);
        var marketplace = ParseObject(root, marketplacePath, errors);
        if (marketplace?["plugins"] is not JsonArray entries)
        {
            errors.Add($"{relativePath}: plugins must be an array");
            return;
        }

        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entry in entries.OfType<JsonObject>())
        {
            var name = GetString(entry, "name");
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add($"{relativePath}: each plugin entry needs a name");
                continue;
            }

            names.Add(name);
            if (codex)
            {
                var source = GetObject(entry, "source");
                var policy = GetObject(entry, "policy");
                if (!string.Equals(GetString(source, "source"), "local", StringComparison.Ordinal) ||
                    !string.Equals(GetString(source, "path"), $"./plugins/{name}", StringComparison.Ordinal))
                {
                    errors.Add($"{relativePath}: {name} has an invalid Codex source");
                }

                if (!string.Equals(GetString(policy, "installation"), "AVAILABLE", StringComparison.Ordinal) ||
                    !string.Equals(GetString(policy, "authentication"), "ON_INSTALL", StringComparison.Ordinal))
                {
                    errors.Add($"{relativePath}: {name} has an invalid Codex policy");
                }

                if (string.IsNullOrWhiteSpace(GetString(entry, "category")))
                {
                    errors.Add($"{relativePath}: {name} needs a category");
                }
            }
            else if (!string.Equals(GetString(entry, "source"), $"./plugins/{name}", StringComparison.Ordinal))
            {
                errors.Add($"{relativePath}: {name} has an invalid Claude source");
            }
        }

        if (!names.SetEquals(plugins))
        {
            errors.Add($"{relativePath}: entries must match plugin directories");
        }
    }

    private static string? ReadFile(string root, string path, ICollection<string> errors)
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            errors.Add($"{Path.GetRelativePath(root, path)}: {exception.Message}");
            return null;
        }
    }

    private static JsonObject? ParseObject(string root, string path, ICollection<string> errors)
    {
        try
        {
            var document = JsonNode.Parse(File.ReadAllText(path));
            if (document is JsonObject objectDocument) return objectDocument;

            errors.Add($"{Path.GetRelativePath(root, path)}: expected a JSON object");
            return null;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            errors.Add($"{Path.GetRelativePath(root, path)}: {exception.Message}");
            return null;
        }
    }

    private static JsonObject? GetObject(JsonObject? value, string property)
    {
        return value?[property] as JsonObject;
    }

    private static string? GetString(JsonObject? value, string property)
    {
        return value?[property] is JsonValue jsonValue && jsonValue.TryGetValue<string>(out var text) ? text : null;
    }

    private static string? ReadFrontmatterValue(IEnumerable<string> lines, string key)
    {
        var prefix = $"{key}:";
        return lines.FirstOrDefault(line => line.StartsWith(prefix, StringComparison.Ordinal))?[prefix.Length..]?.Trim();
    }

    [GeneratedRegex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-[0-9A-Za-z.-]+)?(?:\+[0-9A-Za-z.-]+)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex SemanticVersionRegex();
}