using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GithubStarSearch;

public class Indexer
{
    private readonly ILogger<Indexer> _logger;

    public Indexer(ILogger<Indexer> logger)
    {
        _logger = logger;
    }

    [Function("Indexer")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            
        }
    }
}