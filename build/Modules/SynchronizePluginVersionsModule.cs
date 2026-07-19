using System.Text.RegularExpressions;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Synchronizes the GitVersion release version into plugin manifests.
/// </summary>
[DependsOn<ResolveVersioningModule>]
public sealed partial class SynchronizePluginVersionsModule : Module<SynchronizePluginVersionsResult>
{
    protected override async Task<SynchronizePluginVersionsResult?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        var root = context.Git().RootDirectory.Path;
        var hasChanges = false;

        foreach (var pluginDirectory in Directory.GetDirectories(Path.Combine(root, "plugins")).OrderBy(path => path, StringComparer.Ordinal))
        {
            hasChanges |= await SynchronizeAsync(Path.Combine(pluginDirectory, "plugin.json"), versioning.Version, cancellationToken);
            hasChanges |= await SynchronizeAsync(Path.Combine(pluginDirectory, ".codex-plugin", "plugin.json"), versioning.Version, cancellationToken);
        }

        return new SynchronizePluginVersionsResult
        {
            Version = versioning.Version,
            HasChanges = hasChanges
        };
    }

    private static async Task<bool> SynchronizeAsync(string path, string version, CancellationToken cancellationToken)
    {
        var content = await File.ReadAllTextAsync(path, cancellationToken);
        var versionProperty = VersionProperty();
        versionProperty.IsMatch(content).ShouldBeTrue($"{path} must contain a version property");
        var updated = versionProperty.Replace(content, match => $"{match.Groups[1].Value}{version}{match.Groups[2].Value}", 1);

        if (string.Equals(updated, content, StringComparison.Ordinal)) return false;

        await File.WriteAllTextAsync(path, updated, cancellationToken);
        return true;
    }

    [GeneratedRegex("""("version"\s*:\s*")[^"]*(")""", RegexOptions.CultureInvariant)]
    private static partial Regex VersionProperty();
}

public sealed record SynchronizePluginVersionsResult
{
    /// <summary>
    ///     GitVersion release version written to the manifests.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    ///     Indicates whether the synchronization changed any manifest.
    /// </summary>
    public required bool HasChanges { get; init; }
}