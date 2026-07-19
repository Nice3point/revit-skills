using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Git.Options;
using ModularPipelines.Modules;
using ModularPipelines.Options;

namespace Build.Modules;

/// <summary>
///     Resolves the marketplace release version from Git history.
/// </summary>
public sealed class ResolveVersioningModule : Module<ResolveVersioningResult>
{
    protected override async Task<ResolveVersioningResult?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioning = await CreateFromGitVersioningAsync(context);

        context.Summary.KeyValue("Build", "Version", versioning.Version);
        return versioning;
    }

    private static async Task<ResolveVersioningResult> CreateFromGitVersioningAsync(IModuleContext context)
    {
        var gitVersioning = await context.Git().Versioning.GetGitVersioningInformation();

        return new ResolveVersioningResult
        {
            Version = gitVersioning.SemVer!,
            PreviousVersion = await FetchPreviousVersionAsync(context)
        };
    }

    private static async Task<string> FetchPreviousVersionAsync(IModuleContext context)
    {
        var describeResult = await context.Git().Commands.Describe(
            new GitDescribeOptions
            {
                Tags = true,
                Abbrev = "0",
                Arguments = ["HEAD^"]
            },
            new CommandExecutionOptions
            {
                ThrowOnNonZeroExitCode = false,
                LogSettings = CommandLoggingOptions.Silent
            });

        var previousTag = describeResult.StandardOutput.Trim();
        if (!string.IsNullOrWhiteSpace(previousTag)) return previousTag;

        var revisionResult = await context.Git().Commands.RevList(
            new GitRevListOptions
            {
                MaxParents = "0",
                MaxCount = "1",
                Pretty = "format:%H",
                Arguments = ["HEAD"],
                NoCommitHeader = true
            },
            new CommandExecutionOptions
            {
                LogSettings = CommandLoggingOptions.Silent
            });

        return revisionResult.StandardOutput.Trim();
    }
}

public sealed record ResolveVersioningResult
{
    /// <summary>
    ///     Marketplace release version.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    ///     Previous marketplace release tag or initial commit.
    /// </summary>
    public required string PreviousVersion { get; init; }
}