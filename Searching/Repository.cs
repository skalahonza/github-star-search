namespace GithubStarSearch.Searching;

public class Repository
{
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

    public static string ComputeRepositoryId(string starredBy, long id)
    {
        // having unique id composed of owner and slug is not enough 
        // because the same repository can be starred by multiple users
        // which would result in multiple documents with the same id
        // we need to be able to filter repositories by the user who starred them
        // A document identifier can be of type integer or string, only composed of alphanumeric characters (a-z A-Z 0-9), hyphens (-) and underscores (_).
        return $"{starredBy}-{id}";
    }
}