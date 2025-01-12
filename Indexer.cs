using GithubStarSearch.Searching;
using Octokit;
using Repository = GithubStarSearch.Searching.Repository;

namespace GithubStarSearch;

/// <summary>
/// Background worker that iteratively indexes repositories.
/// </summary>
public class Indexer(ILogger<Indexer> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromDays(1));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<SearchService>();
                var repositories = await service.GetRepositories();
                var github = CreateClient();

                foreach (var repository in repositories.Results)
                {
                    var readme = await GetReadme(repository, github);
                    logger.LogInformation("Updating README for {Owner}/{Slug}", repository.Owner, repository.Slug);
                    repository.Readme = readme;
                }

                await service.UpdateRepositories(repositories.Results);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while indexing repositories");
            }
        } while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken));
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
        var readme = await client.Repository.Content.GetReadme(repository.Owner, repository.Slug);
        logger.LogDebug("Readme content: {Readme}", readme.Content);
        return readme.Content;
    }
}