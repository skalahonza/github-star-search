using System.Text.Json.Serialization;
using Meilisearch;
using Microsoft.Extensions.Options;

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
    public required Repository Formatted { get; init; }
}

public class SearchOptions
{
    public required string MeilisearchUrl { get; set; }
    public required string ApiKey { get; set; }
    public required string RepositoriesIndexName { get; set; } = "repositories";
    public required string PrimaryKey { get; set; } = "id";
    public required int CropLength { get; set; } = 32;
}

public class SearchService(IOptions<SearchOptions> searchSettings, MeilisearchClient client)
{
    public async Task IndexRepositories(IEnumerable<Repository> repositories)
    {
        var index = client.Index(searchSettings.Value.RepositoriesIndexName);
        
        // Fire-and-Forget
        var task = await index.AddDocumentsAsync(repositories);
    }
    
    public async Task<IReadOnlyCollection<FormattedSearchableRepository>> SearchRepositories(string starredBy, string term)
    {
        var index = client.Index(searchSettings.Value.RepositoriesIndexName);
        
        var searchFilterConditions = new List<string>
        {
            $"{nameof(Repository.StarredBy).ToCamelCase()} =" +
            $" {starredBy}"
        };
        
        // Set the query with filter and additional settings
        var searchQuery = new SearchQuery
        {
            Filter = searchFilterConditions,
            AttributesToHighlight = [
                nameof(Repository.Slug).ToCamelCase(),
                nameof(Repository.Owner).ToCamelCase(),
                nameof(Repository.Description).ToCamelCase()],
            AttributesToCrop = [nameof(Repository.Description).ToCamelCase()],
            CropLength = searchSettings.Value.CropLength
        };

        var results = await index.SearchAsync<FormattedSearchableRepository>(term, searchQuery);
        return results.Hits;
    }
}

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
        {
            return str;
        }

        return char.ToLowerInvariant(str[0]) + str[1..];
    }
}

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddMeilisearch(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<SearchOptions>()
            .Bind(builder.Configuration.GetSection("Meilisearch"));
        
        builder.Services.AddScoped<SearchService>();
        builder.Services.AddScoped<MeilisearchMessageHandler>();
        builder.Services.AddHttpClient(nameof(MeilisearchClient), (serviceProvider, client) =>
        {
            var url = serviceProvider.GetRequiredService<IOptions<SearchOptions>>().Value.MeilisearchUrl;
            client.BaseAddress = new Uri(url);
        }).AddTypedClient<MeilisearchClient>(
            (client, serviceProvider) =>
            {
                var apiKey = serviceProvider.GetRequiredService<IOptions<SearchOptions>>().Value.ApiKey;
                return new MeilisearchClient(client, apiKey);
            });
        return builder;
    }
}