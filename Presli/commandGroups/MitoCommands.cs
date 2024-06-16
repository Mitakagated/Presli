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
    public RouletteGame RouletteGame { get; set; }

    [SlashCommand("kolko_pari_imash", "Можеш да видиш колко пари имаш чрез тази команда")]
    public async Task ViewCurrency(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var UserCurrency = await DatabaseHelper.GetCurrencyInfo(ctx.User.Id);
        var response = $"Ти имаш {UserCurrency.Mito} mito, и {UserCurrency.BettingCurrency} betting currency";
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent(response));
    }

    [SlashCommand("roulette", "Игра на рулетка idk help me")]
    public async Task PlayRoulette(InteractionContext ctx,[Option("Числа", "Числа за залагане")] RouletteGame.Choice choice, [Option("Mito", "Слагай парите на масата")] long mito)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var userMito = await DatabaseHelper.RemoveMito(ctx.User.Id, mito);
        var potentialWin = mito;
        if (userMito <= 0)
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"Нямаш достатъчно Mito: {userMito}"));
        }
        else
        {
            await RouletteGame.PullNumber(choice, ctx);
            if (RouletteGame.foundNum)
            {
                potentialWin = choice switch
                {
                    RouletteGame.Choice.One => potentialWin + potentialWin * 35,
                    RouletteGame.Choice.Two => potentialWin + potentialWin * 17,
                    RouletteGame.Choice.Three => potentialWin + potentialWin * 11,
                    RouletteGame.Choice.Six => potentialWin + potentialWin * 5,
                    RouletteGame.Choice.Black => potentialWin + potentialWin,
                    RouletteGame.Choice.Red => potentialWin + potentialWin,
                    RouletteGame.Choice.Even => potentialWin + potentialWin,
                    RouletteGame.Choice.Odd => potentialWin + potentialWin,
                    RouletteGame.Choice.OneToTwelve => potentialWin + potentialWin * 2,
                    RouletteGame.Choice.ThirteenToTwentyFour => potentialWin + potentialWin * 2,
                    RouletteGame.Choice.TwentyfiveToThirtySix => potentialWin + potentialWin * 2,
                    RouletteGame.Choice.FirstHalf => potentialWin + potentialWin,
                    RouletteGame.Choice.SecondHalf => potentialWin + potentialWin,
                    _ => mito,
                };
                userMito = await DatabaseHelper.AddMito(ctx.User.Id, potentialWin);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Падна се {RouletteGame.pulledNum}, и взе {potentialWin}, което прави общо {userMito} Mito."));
            }
            else
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Падна се {RouletteGame.pulledNum}, и не взе нищо :XD:"));
            }
        }
    }
}
