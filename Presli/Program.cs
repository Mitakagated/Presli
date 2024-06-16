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
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Options;

var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "--with-deps", "firefox" });
if (exitCode != 0)
{
    throw new Exception($"Playwright has exited with code {exitCode}");
}

var builder = new HostApplicationBuilder(args);

DotNetEnv.Env.TraversePath().Load();

builder.Services.AddHostedService<Bot>();
builder.Services.AddSingleton<DiscordClient>();
builder.Services.AddSingleton(new DiscordConfiguration
{
    Token = Environment.GetEnvironmentVariable("token"),
    TokenType = TokenType.Bot,
    AutoReconnect = true,
    Intents = DiscordIntents.All,
    MinimumLogLevel = LogLevel.Debug
});
builder.Services.AddSingleton(new InteractivityConfiguration
{
    Timeout = TimeSpan.FromMinutes(2)
});

builder.Services.AddLavalink();

builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<QueuedLavalinkPlayerOptions>>().Value);

builder.Build().Run();

public sealed class Bot : BackgroundService
{
    private readonly DiscordClient _client;
    private readonly IAudioService _audioService;
    private readonly IOptions<QueuedLavalinkPlayerOptions> _options;

    public Bot(DiscordClient client, IAudioService audioService, IOptions<QueuedLavalinkPlayerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(options);

        _client = client;
        _audioService = audioService;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.UseInteractivity();
        var slash = _client.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = new ServiceCollection().AddSingleton<PlayerHelper>(new PlayerHelper(_audioService, _options))
                                              .AddSingleton<IAudioService>(_audioService)
                                              .AddSingleton<WebScrapingHelper>()
                                              .AddSingleton<RouletteGame>()
                                              .BuildServiceProvider()
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
