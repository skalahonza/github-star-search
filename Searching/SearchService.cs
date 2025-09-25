using System.Text;
using Meilisearch;
using Meilisearch.QueryParameters;
using Microsoft.Extensions.Options;

namespace GithubStarSearch.Searching;

// We need to get _formated part of the response, but the dotnet SDK does not implement it.
// https://github.com/meilisearch/meilisearch-dotnet/issues/315

public record SearchOptions
{
    public string HighlightPreTag { get; init; } = "<em>";
    public string HighlightPostTag { get; init; } = "</em>";
}

public class SearchService(
    IOptions<MeilisearchOptions> searchOptions,
    MeilisearchClient client,
    ILogger<SearchService> logger)
{
    public async Task IndexRepositories(IEnumerable<Repository> repositories)
    {
        var index = client.Index(searchOptions.Value.RepositoriesIndexName);

        var task = await index.AddDocumentsAsync(repositories);

        var info = await index.WaitForTaskAsync(task.TaskUid);
        if (info.Status != TaskInfoStatus.Succeeded)
        {
            var builder = new StringBuilder();
            foreach (var (key, value) in info.Error)
            {
                builder.AppendLine($"{key}: {value}");
            }

            logger.LogError("Failed to index repositories: {Error}", builder.ToString());
        }
        else
        {
            logger.LogInformation("Indexing succeeded in {Duration}", info.Duration);
        }
    }

    public async Task RemoveRepositories(IEnumerable<Repository> repositories)
    {
        var index = client.Index(searchOptions.Value.RepositoriesIndexName);
        var task = await index.DeleteDocumentsAsync(repositories.Select(x => x.Id));

        var info = await index.WaitForTaskAsync(task.TaskUid);
        if (info.Status != TaskInfoStatus.Succeeded)
        {
            var builder = new StringBuilder();
            foreach (var (key, value) in info.Error)
            {
                builder.AppendLine($"{key}: {value}");
            }

            logger.LogError("Failed to remove repositories: {Error}", builder.ToString());
        }
        else
        {
            logger.LogInformation("Removing succeeded in {Duration}", info.Duration);
        }
    }

    public async Task<IReadOnlyCollection<Repository>> SearchRepositories(string starredBy,
        string term,
        SearchOptions options)
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
            AttributesToHighlight =
            [
                nameof(Repository.Slug).ToCamelCase(),
                nameof(Repository.Owner).ToCamelCase(),
                nameof(Repository.Description).ToCamelCase(),
                nameof(Repository.Readme).ToCamelCase()
            ],
            AttributesToCrop = [nameof(Repository.Description).ToCamelCase(), nameof(Repository.Readme).ToCamelCase()],
            CropLength = searchOptions.Value.CropLength,
            HighlightPreTag = options.HighlightPreTag,
            HighlightPostTag = options.HighlightPostTag
        };

        var results = await index.SearchAsync<FormattedSearchableRepository>(term, searchQuery);
        return results.Hits?
            // todo I dont know how to correctly highlight <em> at the moment
            .Select(x => x.Formatted ?? x)
            .ToList() ?? [];
    }

    public async Task<ResourceResults<IEnumerable<Repository>>> GetRepositories(int limit, int offset)
    {
        await MakeSureIndexExists();
        await SetupIndex();

        var index = client.Index(searchOptions.Value.RepositoriesIndexName);
        return await index.GetDocumentsAsync<Repository>(new DocumentsQuery()
        {
            Limit = limit,
            Offset = offset,
        });
    }

    public async IAsyncEnumerable<Repository> GetAllRepositories()
    {
        var index = client.Index(searchOptions.Value.RepositoriesIndexName);
        var offset = 0;
        ResourceResults<IEnumerable<Repository>>? result;
        do
        {
            result = await index.GetDocumentsAsync<Repository>(new DocumentsQuery { Offset = offset });
            foreach (var repository in result.Results)
            {
                yield return repository;
            }

            offset += result.Results.Count();
        } while (result.Results.Any());
    }

    public async Task<bool> IsIndexed(string githubUsername)
    {
        var index = client.Index(searchOptions.Value.RepositoriesIndexName);
        var searchFilterConditions = new List<string>
        {
            $"{nameof(Repository.StarredBy).ToCamelCase()} = {githubUsername}"
        };

        // Set the query with filter and additional settings
        var query = new DocumentsQuery
        {
            Filter = searchFilterConditions,
        };

        var results = await index.GetDocumentsAsync<Repository>(query);
        return results.Total > 0;
    }

    /// <summary>
    /// Updates repositories asynchronously.
    /// It may take a while for the changes to be reflected in the search results.
    /// </summary>
    public async Task UpdateRepositories(IEnumerable<Repository> repositories)
    {
        var index = client.Index(searchOptions.Value.RepositoriesIndexName);
        await index.UpdateDocumentsAsync(repositories);
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
                string.Equals(index.Uid, searchOptions.Value.RepositoriesIndexName,
                    StringComparison.OrdinalIgnoreCase)))
        {
            logger.LogInformation("Index {IndexName} does not exist. Creating one",
                searchOptions.Value.RepositoriesIndexName);
            await client.CreateIndexAsync(searchOptions.Value.RepositoriesIndexName, searchOptions.Value.PrimaryKey);
            logger.LogInformation("Index {IndexName} created", searchOptions.Value.RepositoriesIndexName);
        }
        else
        {
            logger.LogInformation("Index {IndexName} already exists", searchOptions.Value.RepositoriesIndexName);
        }
    }
}