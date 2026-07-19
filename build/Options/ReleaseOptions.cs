namespace Build.Options;

/// <summary>
///     Plugin release options.
/// </summary>
[Serializable]
public sealed record ReleaseOptions
{
    /// <summary>
    ///     Branch that receives release pull requests and tags.
    /// </summary>
    public string BaseBranch { get; init; } = "main";

    /// <summary>
    ///     Branch used for generated version updates.
    /// </summary>
    public string VersionSyncBranch { get; init; } = "automated/plugin-version-sync";
}