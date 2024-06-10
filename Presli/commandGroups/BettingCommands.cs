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
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("Не е направено все още."));
    }
}
