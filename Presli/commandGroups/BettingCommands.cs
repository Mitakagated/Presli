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

namespace Presli.commandGroups;
public class BettingCommands : ApplicationCommandModule
{
    [SlashCommand("bet", "betting commands")]
    public async Task Bet(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("Не е направено все още."));
    }
}
