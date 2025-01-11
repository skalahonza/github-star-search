﻿using Meilisearch;
using Microsoft.Extensions.Options;

namespace GithubStarSearch.Searching;

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