using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using Presli.commandGroups;
using Microsoft.Extensions.DependencyInjection;
using Presli.Models;
using Presli.Classes;
using Microsoft.Extensions.Hosting;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.Logging;

var builder = new HostApplicationBuilder(args);

DotNetEnv.Env.TraversePath().Load();

builder.Services.AddHostedService<Bot>();
builder.Services.AddSingleton<DiscordClient>();
builder.Services.AddSingleton(new DiscordConfiguration
{
    Token = Environment.GetEnvironmentVariable("token"),
    TokenType = TokenType.Bot,
    AutoReconnect = true,
    Intents = DiscordIntents.All
});
builder.Services.AddSingleton(new InteractivityConfiguration
{
    Timeout = TimeSpan.FromMinutes(2)
});

builder.Services.AddLavalink();

builder.Services.AddLogging(s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Build().Run();

file sealed class Bot : BackgroundService
{
    private readonly DiscordClient _client;
    private readonly IAudioService _audioService;

    public Bot(DiscordClient client, IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(audioService);

        _client = client;
        _audioService = audioService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.UseInteractivity();
        var slash = _client.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = new ServiceCollection().AddLavalink().BuildServiceProvider()
        });
        slash.RegisterCommands<funCommands>();
        slash.RegisterCommands<musicCommands>();
        slash.RegisterCommands<MitoCommands>();
        slash.RegisterCommands<BettingCommands>();

        await _client.ConnectAsync().ConfigureAwait(false);

        var readyTaskCompletionSource = new TaskCompletionSource();

        Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            readyTaskCompletionSource.TrySetResult();
            return Task.CompletedTask;
        }

        _client.Ready += OnClientReady;
        await readyTaskCompletionSource.Task.ConfigureAwait(false);
        _client.Ready -= OnClientReady;
    }
}
