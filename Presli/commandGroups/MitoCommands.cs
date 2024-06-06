using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Presli.Classes;
using Presli.Models;

namespace Presli.commandGroups;
public class MitoCommands : ApplicationCommandModule
{
    [SlashCommand("kolko_pari_imash", "Можеш да видиш колко пари имаш чрез тази команда")]
    public async Task ViewCurrency(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var UserCurrency = await DatabaseHelper.GetCurrencyInfo(ctx.User.Id);
        var response = $"Ти имаш {UserCurrency.Mito} mito, и {UserCurrency.BettingCurrency} betting currency";
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent(response));
    }
}
