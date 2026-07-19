using Build.Modules;
using Build.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.Extensions;

var builder = Pipeline.CreateBuilder();

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services.AddOptions<ReleaseOptions>().Bind(builder.Configuration.GetSection("Release"));

if (args.Length == 0)
{
    builder.Services.AddModule<ValidateMarketplaceModule>();
    builder.Services.AddModule<ValidateSkillsModule>();
}

if (args.Contains("version"))
{
    builder.Services.AddModule<ResolveVersioningModule>();
}

if (args.Contains("sync"))
{
    builder.Services.AddModule<CreateVersionSyncPullRequestModule>();
}

if (args.Contains("release"))
{
    builder.Services.AddModule<PublishPluginModule>();
}

await builder.Build().RunAsync();