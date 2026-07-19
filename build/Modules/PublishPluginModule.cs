using Microsoft.Extensions.Logging;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Git.Options;
using ModularPipelines.Modules;
using ModularPipelines.Options;

namespace Build.Modules;

/// <summary>
///     Creates and pushes the marketplace release.
/// </summary>
[DependsOn<VerifyPluginVersionsModule>]
public sealed class PublishPluginModule : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<VerifyPluginVersionsModule>();
        var versioning = versioningResult.ValueOrDefault!;

        var existingTag = await context.Git().Commands.Tag(
            new GitTagOptions
            {
                Arguments = ["-l", versioning.Version]
            },
            new CommandExecutionOptions
            {
                LogSettings = CommandLoggingOptions.Silent
            },
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(existingTag.StandardOutput))
        {
            context.Logger.LogInformation("Tag {Tag} already exists, skipping creation", versioning.Version);
            return;
        }

        context.Logger.LogInformation("Creating release tag {Tag}", versioning.Version);
        await context.Git().Commands.Tag(
            new GitTagOptions
            {
                TagName = versioning.Version
            },
            token: cancellationToken);

        context.Logger.LogInformation("Pushing release tag {Tag} to origin", versioning.Version);
        await context.Git().Commands.Push(
            new GitPushOptions
            {
                Arguments = ["origin", versioning.Version]
            },
            token: cancellationToken);
    }
}