using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
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

namespace Presli
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("token"),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.All
            });
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var services = new ServiceCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .BuildServiceProvider();
            
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "lavalink",
                Port = 2333
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            
            var lavalink = discord.UseLavalink();
            var slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = services
            });
            slash.RegisterCommands<funCommands>();
            slash.RegisterCommands<musicCommands>();
            slash.RegisterCommands<MitoCommands>();
            slash.RegisterCommands<BettingCommands>();
            await discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            
            await Task.Delay(-1);
        }
        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
