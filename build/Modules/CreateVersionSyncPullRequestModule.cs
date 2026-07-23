using Build.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Git.Options;
using ModularPipelines.GitHub.Attributes;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using Octokit;

namespace Build.Modules;

/// <summary>
///     Stamps changed plugin versions, commits them to the synchronization branch, and opens the corresponding pull request.
/// </summary>
[DependsOn<SynchronizePluginVersionsModule>]
[SkipIfNoGitHubToken]
public sealed class CreateVersionSyncPullRequestModule(IOptions<ReleaseOptions> releaseOptions) : Module
{
    private const string CommitMessage = "Stamp plugin versions";
    private const string PullRequestBody = "Automated GitVersion synchronization for changed marketplace content.";

    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var synchronizationResult = await context.GetModule<SynchronizePluginVersionsModule>();
        var synchronization = synchronizationResult.ValueOrDefault!;

        if (!synchronization.HasChanges)
        {
            context.Logger.LogInformation("Plugin versions are current");
            return;
        }

        var options = releaseOptions.Value;
        await CommitAndPushAsync(context, options, cancellationToken);
        await CreateOrUpdatePullRequestAsync(context, options);
    }

    private static async Task CommitAndPushAsync(IModuleContext context, ReleaseOptions options, CancellationToken cancellationToken)
    {
        await context.Git().Commands.Checkout(
            new GitCheckoutOptions(options.VersionSyncBranch, create: true)
            {
                Force = true
            },
            token: cancellationToken);

        await context.Git().Commands.Config(
            new GitConfigOptions
            {
                Arguments = ["user.name", "github-actions[bot]"]
            },
            token: cancellationToken);

        await context.Git().Commands.Config(
            new GitConfigOptions
            {
                Arguments = ["user.email", "41898282+github-actions[bot]@users.noreply.github.com"]
            },
            token: cancellationToken);

        await context.Git().Commands.Add(
            new GitAddOptions
            {
                Arguments = ["plugins/*/plugin.json", "plugins/*/.codex-plugin/plugin.json"],
            },
            new CommandExecutionOptions
            {
                WorkingDirectory = context.Git().RootDirectory
            },
            token: cancellationToken);

        await context.Git().Commands.Commit(
            new GitCommitOptions
            {
                Arguments = ["-m", CommitMessage]
            },
            token: cancellationToken);

        await context.Git().Commands.Push(
            new GitPushOptions
            {
                Arguments = ["--force", "origin", options.VersionSyncBranch]
            },
            token: cancellationToken);
    }

    private static async Task CreateOrUpdatePullRequestAsync(IModuleContext context, ReleaseOptions options)
    {
        var repositoryInfo = context.GitHub().RepositoryInfo;
        var client = new GitHubClient(new ProductHeaderValue("revit-skills-version-sync"))
        {
            Credentials = new Credentials(context.GitHub().EnvironmentVariables.Token)
        };

        var pullRequests = await client.PullRequest.GetAllForRepository(repositoryInfo.Owner, repositoryInfo.RepositoryName, new PullRequestRequest
        {
            State = ItemStateFilter.Open
        });

        var existingPullRequest = pullRequests.SingleOrDefault(pullRequest => string.Equals(pullRequest.Head.Ref, options.VersionSyncBranch, StringComparison.Ordinal));
        if (existingPullRequest is null)
        {
            context.Logger.LogInformation("Opening version synchronization pull request");
            await client.PullRequest.Create(repositoryInfo.Owner, repositoryInfo.RepositoryName, new NewPullRequest(CommitMessage, options.VersionSyncBranch, options.BaseBranch)
            {
                Body = PullRequestBody
            });

            return;
        }

        context.Logger.LogInformation("Updating version synchronization pull request #{PullRequestNumber}", existingPullRequest.Number);
        await client.PullRequest.Update(
            repositoryInfo.Owner,
            repositoryInfo.RepositoryName,
            existingPullRequest.Number,
            new PullRequestUpdate
            {
                Title = CommitMessage,
                Body = PullRequestBody
            });
    }
}