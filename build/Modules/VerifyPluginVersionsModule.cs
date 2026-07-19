using System.Text.Json.Nodes;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Ensures committed plugin manifests match the GitVersion release version.
/// </summary>
[DependsOn<ResolveVersioningModule>]
public sealed class VerifyPluginVersionsModule : Module<ResolveVersioningResult>
{
    protected override async Task<ResolveVersioningResult?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;
        var root = context.Git().RootDirectory.Path;

        foreach (var manifestPath in Directory.GetFiles(Path.Combine(root, "plugins"), "plugin.json", SearchOption.AllDirectories))
        {
            var manifest = JsonNode.Parse(await File.ReadAllTextAsync(manifestPath, cancellationToken)) as JsonObject;
            var version = manifest?["version"]?.GetValue<string>();

            version.ShouldBe(versioning.Version, $"{Path.GetRelativePath(root, manifestPath)} must be synchronized before release");
        }

        return versioning;
    }
}