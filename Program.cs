using Combination.Components;
using MudBlazor.Services;
using MongoDB.Driver;
using Microsoft.AspNetCore.SignalR;
using Combination.Hubs;
using Combination.Services;
using Microsoft.AspNetCore.Components;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddHttpClient();

// Add SignalR
builder.Services.AddSignalR();

// Add MongoDB clients and databases
var connectionString1 = builder.Configuration.GetSection("MongoDB1:ConnectionString").Value;
var databaseName1 = builder.Configuration.GetSection("MongoDB1:DatabaseName").Value;
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString1));
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName1);
});

var connectionString2 = builder.Configuration.GetSection("MongoDB2:ConnectionString").Value;
var databaseName2 = builder.Configuration.GetSection("MongoDB2:DatabaseName").Value;
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var existingClient = serviceProvider.GetService<IMongoClient>();
    if (existingClient.Settings.Server.Host == new MongoUrl(connectionString2).Server.Host)
    {
        return existingClient;
    }
    return new MongoClient(connectionString2);
});
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName2);
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register Services
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<LeaderboardService>();

builder.Services.AddSingleton<QuestionService>();
builder.Services.AddSingleton<MongoDBService>();

// Add HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add routing and SignalR hub
app.UseRouting();
app.UseAntiforgery();
app.MapHub<RoomHub>("/roomHub");
app.MapHub<QuizHub>("/quizhub");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();