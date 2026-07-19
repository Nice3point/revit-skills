using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Validates skill content against the authoring standard in AGENTS.md.
/// </summary>
public sealed partial class ValidateSkillsModule : SyncModule
{
    private const int MaxNameLength = 64;
    private const int MaxDescriptionLength = 1024;
    private const int MinDescriptionLength = 20;
    private const int MaxBodyLines = 500;
    private const int WarnTokenCount = 5000;
    private const long MaxAssetFileSize = 5 * 1024 * 1024;

    private static readonly string[] AssetDirectories = ["references", "scripts", "assets"];

    protected override void ExecuteModule(IModuleContext context, CancellationToken cancellationToken)
    {
        var root = context.Git().RootDirectory.Path;
        var pluginsRoot = Path.Combine(root, "plugins");
        var errors = new List<string>();
        var warnings = new List<string>();

        var skillPlugins = MapSkillsToPlugins(pluginsRoot);

        foreach (var (skillName, plugin) in skillPlugins.OrderBy(entry => entry.Key, StringComparer.Ordinal))
        {
            var skillDirectory = Path.Combine(pluginsRoot, plugin, "skills", skillName);
            CheckSkill(plugin, skillName, skillDirectory, skillPlugins, errors, warnings);
        }

        foreach (var warning in warnings)
        {
            context.Logger.LogWarning("{Warning}", warning);
        }

        errors.ShouldBeEmpty($"Skill validation failed:{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", errors)}");

        context.Summary.KeyValue("Skills", "Checked", skillPlugins.Count.ToString());
    }

    private static Dictionary<string, string> MapSkillsToPlugins(string pluginsRoot)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pluginDirectory in Directory.GetDirectories(pluginsRoot))
        {
            var skillsDirectory = Path.Combine(pluginDirectory, "skills");
            if (!Directory.Exists(skillsDirectory)) continue;

            var plugin = Path.GetFileName(pluginDirectory);
            foreach (var skillDirectory in Directory.GetDirectories(skillsDirectory))
            {
                if (File.Exists(Path.Combine(skillDirectory, "SKILL.md")))
                {
                    map[Path.GetFileName(skillDirectory)] = plugin;
                }
            }
        }

        return map;
    }

    private static void CheckSkill(string plugin, string skillName, string skillDirectory, IReadOnlyDictionary<string, string> skillPlugins, List<string> errors, List<string> warnings)
    {
        var location = $"plugins/{plugin}/skills/{skillName}";
        var content = File.ReadAllText(Path.Combine(skillDirectory, "SKILL.md")).Replace("\r\n", "\n");
        var lines = content.Split('\n');

        if (lines.Length == 0 || lines[0] != "---")
        {
            errors.Add($"{location}: SKILL.md must start with YAML frontmatter");
            return;
        }

        var closingIndex = Array.IndexOf(lines, "---", 1);
        if (closingIndex < 0)
        {
            errors.Add($"{location}: frontmatter has no closing '---' marker");
            return;
        }

        var frontmatter = lines[1..closingIndex];
        var body = string.Join("\n", lines[(closingIndex + 1)..]);

        CheckName(location, frontmatter, errors);
        CheckDescription(location, frontmatter, errors);
        CheckBody(location, content, body, errors, warnings);
        CheckLinks(location, body, errors);
        CheckInsecureReferences(location, content, errors);
        CheckCrossPluginReferences(location, skillName, skillDirectory, skillPlugins, errors);
        CheckAssets(location, skillDirectory, errors);

        if (ReadScalar(frontmatter, "license") is null)
        {
            warnings.Add($"{location}: frontmatter has no 'license' key (house convention: license: MIT)");
        }
    }

    private static void CheckName(string location, IReadOnlyList<string> frontmatter, ICollection<string> errors)
    {
        var name = ReadScalar(frontmatter, "name");
        if (name is null || name.Length > MaxNameLength || !NameFormatRegex().IsMatch(name))
        {
            errors.Add($"{location}: name must be 1-{MaxNameLength} lowercase kebab-case characters (letters, digits, single hyphens)");
        }
    }

    private static void CheckDescription(string location, IReadOnlyList<string> frontmatter, ICollection<string> errors)
    {
        var description = ReadDescription(frontmatter);
        if (string.IsNullOrWhiteSpace(description))
        {
            errors.Add($"{location}: frontmatter has no description");
        }
        else if (description.Length > MaxDescriptionLength)
        {
            errors.Add($"{location}: description is {description.Length} chars — maximum is {MaxDescriptionLength}");
        }
        else if (description.Length < MinDescriptionLength)
        {
            errors.Add($"{location}: description is only {description.Length} chars — too short to route on");
        }
    }

    private static void CheckBody(string location, string content, string body, ICollection<string> errors, ICollection<string> warnings)
    {
        var bodyLineCount = body.Split('\n').Length;
        if (bodyLineCount > MaxBodyLines)
        {
            errors.Add($"{location}: body is {bodyLineCount} lines — maximum is {MaxBodyLines}; move detail into references/");
        }

        var estimatedTokens = content.Length / 4;
        if (estimatedTokens > WarnTokenCount)
        {
            warnings.Add($"{location}: ~{estimatedTokens} tokens (chars/4) — over {WarnTokenCount}; consider splitting into focused skills");
        }

        if (!body.Contains("```", StringComparison.Ordinal))
        {
            warnings.Add($"{location}: no fenced code block — add a concrete snippet unless this is a pure rules skill");
        }
    }

    private static void CheckLinks(string location, string body, ICollection<string> errors)
    {
        foreach (Match match in FileLinkRegex().Matches(body))
        {
            var target = match.Groups[1].Value;
            if (target.StartsWith("http", StringComparison.OrdinalIgnoreCase)) continue;
            if (target.StartsWith("//", StringComparison.Ordinal)) continue;
            if (target.StartsWith('#')) continue;

            var path = target;
            var fragment = path.IndexOf('#');
            if (fragment >= 0) path = path[..fragment];
            if (path.StartsWith("./", StringComparison.Ordinal)) path = path[2..];
            if (path.Length == 0) continue;

            if (path.StartsWith('/'))
            {
                errors.Add($"{location}: link '{target}' uses an absolute path — links must be relative to the skill directory");
                continue;
            }

            var segments = path.Split('/');
            if (segments.Contains(".."))
            {
                errors.Add($"{location}: link '{target}' escapes the skill directory with '..'");
            }
            else if (segments.Length - 1 > 1)
            {
                errors.Add($"{location}: link '{target}' is more than one directory deep");
            }
        }
    }

    private static void CheckInsecureReferences(string location, string content, ICollection<string> errors)
    {
        foreach (Match match in UrlRegex().Matches(content))
        {
            var url = match.Value.TrimEnd('.', ',', ';', ':', ')', '\'', '"');
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !IsLocalHost(url))
            {
                errors.Add($"{location}: insecure http:// URL (use https://): {url}");
            }
        }

        if (PipeToShellRegex().IsMatch(content))
        {
            errors.Add($"{location}: pipe-to-shell pattern (curl/wget piped into a shell) is not allowed");
        }

        foreach (Match match in ScriptTagRegex().Matches(content))
        {
            if (!IntegrityRegex().IsMatch(match.Value))
            {
                errors.Add($"{location}: external <script> tag without an integrity (SRI) attribute");
            }
        }
    }

    private static void CheckCrossPluginReferences(string location, string skillName, string skillDirectory, IReadOnlyDictionary<string, string> skillPlugins, List<string> errors)
    {
        var ownPlugin = skillPlugins[skillName];
        var files = new List<string> { Path.Combine(skillDirectory, "SKILL.md") };
        var referencesDirectory = Path.Combine(skillDirectory, "references");
        if (Directory.Exists(referencesDirectory))
        {
            files.AddRange(Directory.GetFiles(referencesDirectory, "*.md", SearchOption.AllDirectories));
        }

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            foreach (var (otherSkill, otherPlugin) in skillPlugins)
            {
                if (otherSkill == skillName || otherPlugin == ownPlugin) continue;
                if (Regex.IsMatch(text, $@"(?<![\w-]){Regex.Escape(otherSkill)}(?![\w-])"))
                {
                    errors.Add($"{location}: references '{otherSkill}' from plugin '{otherPlugin}' — name the concrete API instead (plugins install independently)");
                }
            }
        }
    }

    private static void CheckAssets(string location, string skillDirectory, ICollection<string> errors)
    {
        foreach (var assetDirectory in AssetDirectories)
        {
            var directory = Path.Combine(skillDirectory, assetDirectory);
            if (!Directory.Exists(directory)) continue;

            foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(skillDirectory, file).Replace('\\', '/');
                if (relative.Split('/').Length - 1 > 1)
                {
                    errors.Add($"{location}: bundled file '{relative}' is more than one directory deep");
                }

                var length = new FileInfo(file).Length;
                if (length > MaxAssetFileSize)
                {
                    errors.Add($"{location}: bundled file '{relative}' is {length / (1024.0 * 1024.0):F1} MB — maximum is 5 MB");
                }
            }
        }
    }

    private static bool IsLocalHost(string url)
    {
        if (url.Contains("//localhost", StringComparison.OrdinalIgnoreCase)) return true;
        if (url.Contains("//127.0.0.1", StringComparison.Ordinal)) return true;
        return false;
    }

    private static string? ReadScalar(IEnumerable<string> frontmatter, string key)
    {
        var prefix = $"{key}:";
        var line = frontmatter.FirstOrDefault(candidate => candidate.StartsWith(prefix, StringComparison.Ordinal));
        return line?[prefix.Length..]?.Trim();
    }

    private static string ReadDescription(IReadOnlyList<string> frontmatter)
    {
        var index = -1;
        for (var i = 0; i < frontmatter.Count; i++)
        {
            if (frontmatter[i].StartsWith("description:", StringComparison.Ordinal))
            {
                index = i;
                break;
            }
        }

        if (index < 0) return string.Empty;

        var inline = frontmatter[index]["description:".Length..].Trim();
        if (inline.Length > 0 && inline is not (">" or "|" or ">-" or "|-")) return inline;

        var folded = new List<string>();
        for (var i = index + 1; i < frontmatter.Count; i++)
        {
            var line = frontmatter[i];
            if (line.Length > 0 && !char.IsWhiteSpace(line[0])) break; // next top-level key
            if (line.Trim().Length > 0) folded.Add(line.Trim());
        }

        return string.Join(" ", folded);
    }

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex NameFormatRegex();

    [GeneratedRegex(@"\]\(([^)]+)\)")]
    private static partial Regex FileLinkRegex();

    [GeneratedRegex("""https?://[^\s\)\]"'<>;]+""", RegexOptions.IgnoreCase)]
    private static partial Regex UrlRegex();

    [GeneratedRegex(@"(?:curl|wget)\s[^|]*\|\s*(?:ba)?sh\b", RegexOptions.IgnoreCase)]
    private static partial Regex PipeToShellRegex();

    [GeneratedRegex("""(?is)<script\b[^>]*\bsrc\s*=\s*["'][^"']*["'][^>]*>""")]
    private static partial Regex ScriptTagRegex();

    [GeneratedRegex(@"(?i)\bintegrity\s*=")]
    private static partial Regex IntegrityRegex();
}