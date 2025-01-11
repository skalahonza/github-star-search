using System.Text;
using System.Text.Json.Serialization;
using Meilisearch;
using Microsoft.Extensions.Options;
using Index = Meilisearch.Index;

namespace GithubStarSearch.Searching;

public class Repository
{
    public required long Id { get; init; }
    public required string Slug { get; init; }
    public required string Owner { get; init; }
    public required string Url { get; init; }

    /// <summary>
    /// Person who added this repository to their starred list.
    /// </summary>
    public required string StarredBy { get; init; }

    public required string Description { get; init; }
    public required DateTimeOffset UpdatedAt { get; set; }
}

// We need to get _formated part of the response, but the dotnet SDK does not implement it.
// https://github.com/meilisearch/meilisearch-dotnet/issues/315
public class FormattedSearchableRepository : Repository
{
    [JsonPropertyName("_formatted")]
    public Repository? Formatted { get; init; }
}

public class SearchOptions
{
    public required string MeilisearchUrl { get; set; }
    public required string ApiKey { get; set; }
    public required string RepositoriesIndexName { get; set; } = "repositories";
    public required string PrimaryKey { get; set; } = "id";
    public required int CropLength { get; set; } = 32;
}

public class SearchService(
    IOptions<SearchOptions> searchOptions,
    MeilisearchClient client,
    ILogger<SearchService> logger)
{
    public async Task IndexRepositories(IEnumerable<Repository> repositories)
    {
        var index = client.Index(searchOptions.Value.RepositoriesIndexName);
        
        // Fire-and-Forget
        var task = await index.AddDocumentsAsync(repositories);
    }
    
    public async Task<IReadOnlyCollection<FormattedSearchableRepository>> SearchRepositories(string starredBy, string term)
    {
        await MakeSureIndexExists();
        await SetupIndex();
        
        var index = client.Index(searchOptions.Value.RepositoriesIndexName);
        var searchFilterConditions = new List<string>();
        
        if (!string.IsNullOrEmpty(starredBy))
        {
            searchFilterConditions.Add($"{nameof(Repository.StarredBy).ToCamelCase()} = {starredBy}");
        }
        
        // Set the query with filter and additional settings
        var searchQuery = new SearchQuery
        {
            Filter = searchFilterConditions,
            // AttributesToHighlight = [
            //     nameof(Repository.Slug).ToCamelCase(),
            //     nameof(Repository.Owner).ToCamelCase(),
            //     nameof(Repository.Description).ToCamelCase()],
            // AttributesToCrop = [nameof(Repository.Description).ToCamelCase()],
            // CropLength = searchSettings.Value.CropLength
        };

        var results = await index.SearchAsync<FormattedSearchableRepository>(term, searchQuery);
        return results.Hits;
    }

    private async Task SetupIndex()
    {
        logger.LogInformation("Updating Meilisearch search settings");
        var settings = new Settings
        {
            // SearchableAttributes = new[]
            // {
            //     nameof(Repository.Description).ToCamelCase(),
            // },
            FilterableAttributes = new[]
            {
                nameof(Repository.StarredBy).ToCamelCase(),
            },
        };

        var index = await client.GetIndexAsync(searchOptions.Value.RepositoriesIndexName);
        await index.UpdateSettingsAsync(settings);
        logger.LogInformation("Meilisearch search settings updated");
    }

    private async Task MakeSureIndexExists()
    {
        var indexes = await client.GetAllIndexesAsync();
        if (!indexes.Results.Any(index =>
                string.Equals(index.Uid, searchOptions.Value.RepositoriesIndexName, StringComparison.OrdinalIgnoreCase)))
        {
            logger.LogInformation("Index {IndexName} does not exist. Creating one", searchOptions.Value.RepositoriesIndexName);
            await client.CreateIndexAsync(searchOptions.Value.RepositoriesIndexName, searchOptions.Value.PrimaryKey);
            logger.LogInformation("Index {IndexName} created", searchOptions.Value.RepositoriesIndexName);
        }
        else
        {
            logger.LogInformation("Index {IndexName} already exists", searchOptions.Value.RepositoriesIndexName);
        }
    }
}