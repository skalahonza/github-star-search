namespace GithubStarSearch.Searching;

public class MeilisearchOptions
{
    public required string MeilisearchUrl { get; set; }
    public required string ApiKey { get; set; }
    public required string RepositoriesIndexName { get; set; } = "repositories";
    public required string PrimaryKey { get; set; } = "id";
    public required int CropLength { get; set; } = 32;
}