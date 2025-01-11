using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace GithubStarSearch;

public class Onboard(ILogger<Onboard> logger)
{
    public record OnboardUser(string Username);
    
    public record UserOnboarded(string Username);
    
    [Function("Onboard")]
    [OpenApiOperation(operationId: "Run", tags: ["onboard"])]
    [OpenApiRequestBody("application/json", typeof(OnboardUser), Description = "User data", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserOnboarded), Description = "The OK response")]
    public async Task<UserOnboarded> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, [FromBody] OnboardUser user)
    {
        logger.LogInformation("C# HTTP trigger function processed a request");
        logger.LogInformation("User {Username} onboarded successfully", user.Username);
        return new UserOnboarded(user.Username);
    }

}