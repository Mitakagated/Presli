using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Npgsql;
using Presli.Classes;
using Presli.Models;
using static Presli.Classes.WebScrapingHelper;

namespace Presli.commandGroups;
public class BettingCommands : ApplicationCommandModule
{
    public WebScrapingHelper WebScrapingHelper { get; set; }
    public event EventHandler UpdateSearch;

    [SlashCommand("bet", "betting commands")]
    public async Task Bet(InteractionContext ctx, [Option("Server", "Сървъра, в който се намира акаунта")] Regions regions, [Option("SummonerName", "Името на акаунта in-game")] string summonerName, [Option("RiotID", "Riot ID на акаунта (тага след името)")] string riotID)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var carryScore = await WebScrapingHelper.CarryScoreFinder(regions, summonerName, riotID);
        if (carryScore == 0)
        {
            await ctx.EditResponseAsync(new()
            {
                Content = $"Ще те тагна когато имам резултати."
            });

            UpdateSearch += BettingCommands_UpdateSearch;

            async void BettingCommands_UpdateSearch(object? sender, EventArgs e)
            {
                await Bet(ctx, regions, summonerName, riotID).ConfigureAwait(false);
            }

            if (DateTime.Compare(DateTime.UtcNow, WebScrapingHelper._lastSearched.AddSeconds(5)) > 0)
            {
                UpdateSearch?.Invoke(this, EventArgs.Empty);
                UpdateSearch -= BettingCommands_UpdateSearch;
            }
        }
        else
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent($"{carryScore}"));
        }
    }
}
