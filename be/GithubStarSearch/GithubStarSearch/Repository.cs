namespace GithubStarSearch;

public class Repository
{
    public required string Slug { get; init; }
    public required string Owner { get; init; }

    /// <summary>
    /// Person who added this repository to their starred list.
    /// </summary>
    public required string StarredBy { get; init; }

    public required string Title { get; init; }
    public required string Description { get; init; }
}