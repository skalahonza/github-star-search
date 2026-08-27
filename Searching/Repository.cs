namespace GithubStarSearch.Searching;

public class Repository
{
    private DateTimeOffset? _pushedAt;

    public required string Id { get; init; }
    public required string Slug { get; init; }
    public required string Owner { get; init; }
    public required string Url { get; init; }

    /// <summary>
    /// Person who added this repository to their starred list.
    /// </summary>
    public required string StarredBy { get; init; }

    public required string Description { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }

    public string Readme { get; set; } = "";

    /// <summary>
    /// Whether the repository was archived (made read-only) by its owner.
    /// </summary>
    public bool Archived { get; set; }

    /// <summary>
    /// Last time a commit was pushed to the repository.
    /// Null when unknown, e.g. for documents indexed before this field was introduced.
    /// </summary>
    public DateTimeOffset? PushedAt
    {
        get => _pushedAt;
        set
        {
            _pushedAt = value;
            PushedAtUnix = value?.ToUnixTimeSeconds() ?? 0;
        }
    }

    /// <summary>
    /// <see cref="PushedAt"/> in seconds since the unix epoch, 0 when unknown.
    /// Meilisearch can only filter and sort on numbers, so the date is duplicated here.
    /// </summary>
    public long PushedAtUnix { get; set; }

    /// <summary>
    /// True when the repository has not been pushed to since <paramref name="staleBefore"/>.
    /// Repositories with an unknown push date are never considered stale.
    /// </summary>
    public bool IsStale(DateTimeOffset staleBefore) => PushedAt is { } pushedAt && pushedAt < staleBefore;

    public static Repository FromGithubRepository(Octokit.Repository repository, string starredBy) => new()
    {
        Id = ComputeRepositoryId(starredBy, repository.Id),
        Slug = repository.Name,
        Owner = repository.Owner.Login,
        Url = repository.HtmlUrl,
        UpdatedAt = repository.UpdatedAt,
        StarredBy = starredBy,
        Description = repository.Description,
        Archived = repository.Archived,
        PushedAt = repository.PushedAt,
    };

    private static string ComputeRepositoryId(string starredBy, long id)
    {
        // having unique id composed of owner and slug is not enough 
        // because the same repository can be starred by multiple users
        // which would result in multiple documents with the same id
        // we need to be able to filter repositories by the user who starred them
        // A document identifier can be of type integer or string, only composed of alphanumeric characters (a-z A-Z 0-9), hyphens (-) and underscores (_).
        return $"{starredBy}-{id}";
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override bool Equals(object? obj) => obj is Repository other && other.Id == Id;
}
