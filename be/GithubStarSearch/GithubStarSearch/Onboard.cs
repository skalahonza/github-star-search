using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GithubStarSearch;

public class Onboard(ILogger<Onboard> logger)
{
    public record OnboardUser(string Username);

    public record UserOnboarded(string Username, int StarredRepositories);

    [Function("Onboard")]
    [OpenApiOperation(operationId: "Run", tags: ["onboard"])]
    [OpenApiRequestBody("application/json", typeof(OnboardUser), Description = "User data", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(UserOnboarded), Description = "The OK response")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(string), Description = "The Bad Request response")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        using var reader = new StreamReader(req.Body);
        var requestBody = await reader.ReadToEndAsync();
        var onboard = JsonSerializer.Deserialize<OnboardUser>(requestBody, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        if (onboard is null || string.IsNullOrWhiteSpace(onboard.Username))
        {
            return new BadRequestObjectResult("Invalid user data.");
        }

        logger.LogInformation("Onboarding {Username}", onboard.Username);

        logger.LogInformation("Initializing github client");
        var github = new GitHubClient(new ProductHeaderValue("GithubStarSearch"));

        logger.LogInformation("Fetching user {Username} data", onboard.Username);
        var user = await github.User.Get(onboard.Username);

        if (user is null)
        {
            logger.LogWarning("User {Username} not found", onboard.Username);
            return new BadRequestObjectResult("User not found");
        }

        logger.LogInformation("Fetching user {Username} starred repositories", onboard.Username);
        var starred = await github.Activity.Starring.GetAllForUser(onboard.Username);

        foreach (var repository in starred)
        {
            logger.LogInformation("Indexing repository {Repository} starred by {Username}", repository.FullName,
                onboard.Username);
        }


        logger.LogInformation("User {Username} onboarded successfully", onboard.Username);
        return new OkObjectResult(new UserOnboarded(onboard.Username, starred.Count));
    }
}