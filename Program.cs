using Blazored.LocalStorage;
using GithubStarSearch;
using GithubStarSearch.Components;
using GithubStarSearch.Searching;
using MudBlazor;
using MudBlazor.Services;
using Octokit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add serilog
builder.Services.AddSerilog(x => x
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"));

// Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopLeft;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.UseMinimalHttpLogger();

// Application logic services
builder.Services.AddScoped<GitHubClient>(x =>
{
    var configuration = x.GetRequiredService<IConfiguration>();
    var logger = x.GetRequiredService<ILogger<Program>>();
    var token = configuration["Github:FineGrainedToken"];

    var client = new GitHubClient(new ProductHeaderValue("GithubStarSearch"));
    if (string.IsNullOrEmpty(token))
    {
        logger.LogCritical("No Github:FineGrainedToken found in configuration");
    }
    else
    {
        client.Credentials = new Credentials(token);
    }

    return client;
});
builder.AddMeilisearch();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHostedService<RepositoryUpdater>();
builder.Services.AddHostedService<StarsUpdater>();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();