using GithubStarSearch.Searching;
using Octokit;
using Repository = GithubStarSearch.Searching.Repository;

namespace GithubStarSearch;

/// <summary>
/// Periodically checks stars of monitored users.
/// Adds newly starred repositories to the full-text search index.
/// Removes un-starred repositories from the full-text search index.
/// </summary>
public class StarsUpdater(ILogger<StarsUpdater> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private const int RequestLimit = 200;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(5));

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        do
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<SearchService>();
                var github = scope.ServiceProvider.GetRequiredService<GitHubClient>();

                var allRepositories = new Dictionary<string, HashSet<Repository>>();
                await foreach (var repository in service.GetAllRepositories().WithCancellation(ct))
                {
                    allRepositories.TryAdd(repository.StarredBy, []);
                    allRepositories[repository.StarredBy].Add(repository);
                }

                foreach (var (user, currentStars) in allRepositories)
                {
                    logger.LogInformation("Fetching starred repositories for user {User}", user);
                    var starred = await github.Activity.Starring.GetAllForUser(user) ?? [];
                    var githubStars = starred.Select(x => Repository.FromGithubRepository(x, user)).ToHashSet();
                    logger.LogInformation("Fetched {Count} starred repositories for user {User}", starred.Count, user);

                    var newlyStarred = githubStars.Except(currentStars).ToList();
                    logger.LogInformation("{Count} newly starred repositories for user {User}", newlyStarred.Count,
                        user);
                    if (newlyStarred.Any())
                    {
                        await service.IndexRepositories(newlyStarred);
                    }

                    var unStarred = currentStars.Except(githubStars).ToList();
                    logger.LogInformation("{Count} un-starred repositories for user {User}", unStarred.Count, user);
                    if (unStarred.Any())
                    {
                        await service.RemoveRepositories(unStarred);
                    }
                }

                logger.LogInformation("Waiting {Period} for next tick", _timer.Period);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while updating stars");
            }
        } while (!ct.IsCancellationRequested && await _timer.WaitForNextTickAsync(ct));
    }
}