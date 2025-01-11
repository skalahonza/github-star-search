using System.Text.Json.Serialization;

namespace GithubStarSearch.Searching;

public class FormattedSearchableRepository : Repository
{
    [JsonPropertyName("_formatted")]
    public Repository? Formatted { get; init; }
}