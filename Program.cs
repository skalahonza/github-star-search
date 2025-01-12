using Blazored.LocalStorage;
using GithubStarSearch;
using GithubStarSearch.Components;
using GithubStarSearch.Searching;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add serilog
builder.Services.AddSerilog(x => x
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"));

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Application logic services
builder.AddMeilisearch();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHostedService<Indexer>();

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