using GithubStarSearch.Searching;
using Meilisearch;
using Octokit;
using Repository = GithubStarSearch.Searching.Repository;

namespace GithubStarSearch;

/// <summary>
/// Background worker that iteratively indexes repositories.
/// </summary>
public class Indexer(ILogger<Indexer> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private const int RequestLimit = 200;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(5));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var offset = 0;
        do
        {
            try
            {
                logger.LogInformation("Indexing repositories, offset {Offset}", offset);
                var work = await DoWork(offset);
                var count = work.Results.Count();

                logger.LogInformation("Indexed {Count} repositories, total of {Total}", count, work.Total);
                if (count < RequestLimit)
                {
                    count = 0;
                }
                else
                {
                    offset += count;
                }

                logger.LogInformation("Waiting {Period} for next tick", _timer.Period);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while indexing repositories");
            }
        } while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task<ResourceResults<IEnumerable<Repository>>> DoWork(int offset)
    {
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<SearchService>();

        logger.LogInformation("Fetching repositories");
        var repositories = await service.GetRepositories(RequestLimit, offset);
        var github = CreateClient();

        foreach (var repository in repositories.Results)
        {
            var readme = await GetReadme(repository, github);
            logger.LogInformation("Updating README for {Owner}/{Slug}", repository.Owner, repository.Slug);
            repository.Readme = readme;
        }

        logger.LogInformation("Updating repositories");
        await service.UpdateRepositories(repositories.Results);

        return repositories;
    }

    private GitHubClient CreateClient()
    {
        using var scope = serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var token = configuration["Github:FineGrainedToken"];
        return new GitHubClient(new ProductHeaderValue("GithubStarSearch"))
        {
            Credentials = new Credentials(token)
        };
    }

    private async Task<string> GetReadme(Repository repository, GitHubClient client)
    {
        // github limit is 5,000 requests per hour per user

        // Fetch the README
        logger.LogInformation("Fetching README for {Owner}/{Slug}", repository.Owner, repository.Slug);
        try
        {
            var readme = await client.Repository.Content.GetReadme(repository.Owner, repository.Slug);
            logger.LogDebug("Readme content: {Readme}", readme.Content);
            return readme.Content;
        }
        catch (ForbiddenException e)
        {
            logger.LogWarning(e, "Forbidden while fetching README for {Owner}/{Slug}", repository.Owner,
                repository.Slug);
            return "";
        }
        catch (NotFoundException e)
        {
            logger.LogWarning(e, "Readme not found for {Owner}/{Slug}", repository.Owner, repository.Slug);
            return "";
        }
    }
}