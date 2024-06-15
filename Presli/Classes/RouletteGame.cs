using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace Presli.Classes;
public class RouletteGame
{
    private List<int> _nums = new List<int> { 0 ,28, 9, 26, 30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1, 00, 27, 10, 25, 29, 12, 8, 19, 31, 18, 6, 21, 33, 16, 4, 23, 35, 14, 2 };
    private List<int> _blackNums = Enumerable.Range(1, 10).Select(i => i % 2).ToList();
    private static List<int>? _userChoices;
    private char[] separators = new char[] { ' ', ',' };

    public enum Choice
    {
        One,
        Two,
        Three,
        Six,
        Black,
        Red,
        Even,
        Odd,
        OneToTwelve,
        ThirteenToTwentyFour,
        TwentyfiveToThirtySix,
        FirstHalf,
        SecondHalf
    }

    public async Task<bool> PullNumber(Choice choice, InteractionContext ctx)
    {
        var chosenNums = await PrepareList(choice, ctx);
        if (chosenNums!.Contains(_nums[Random.Shared.Next()]))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task<List<int>?> PrepareList(Choice choice, InteractionContext ctx)
    {
        switch (choice)
        {
            case Choice.One:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Напиши едно число за рулетката. Пример: 21"));
                var oneNum = await GetMessageAsync(ctx, separators, 1);
                if (oneNum.Result is not null)
                {
                    return _userChoices;
                }
                else
                {
                    return null;
                }

            case Choice.Two:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Напиши две числа за рулетката. Пример: 21, 15"));
                var twoNum = await GetMessageAsync(ctx, separators, 2);
                if (twoNum.Result is not null)
                {
                    return _userChoices;
                }
                else
                {
                    return null;
                }

            case Choice.Three:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Напиши три числа за рулетката. Пример: 21, 15, 3"));
                var threeNum = await GetMessageAsync(ctx, separators, 3);
                if (threeNum.Result is not null)
                {
                    return _userChoices;
                }
                else
                {
                    return null;
                }

            case Choice.Six:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Напиши шест числа за рулетката. Пример: 21, 15, 3, 6, 1, 29"));
                var sixNum = await GetMessageAsync(ctx, separators, 6);
                if (sixNum.Result is not null)
                {
                    return _userChoices;
                }
                else
                {
                    return null;
                }

            case Choice.Black:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра черните числа."));
                return Enumerable.Range(1, 36).Where(i =>
                {
                    if (i <= 10)
                    {
                        return i % 2 == 0;
                    }
                    else if (i <= 18)
                    {
                        return i % 2 == 1;
                    }
                    else if (i <= 28)
                    {
                        return i % 2 == 0;
                    }
                    else
                    {
                        return i % 2 == 1;
                    }
                }).ToList();

            case Choice.Red:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра червените числа."));
                return Enumerable.Range(1, 36).Where(i =>
                {
                    if (i <= 10)
                    {
                        return i % 2 == 1;
                    }
                    else if (i <= 18)
                    {
                        return i % 2 == 0;
                    }
                    else if (i <= 28)
                    {
                        return i % 2 == 1;
                    }
                    else
                    {
                        return i % 2 == 0;
                    }
                }).ToList();

            case Choice.Even:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра четните числа."));
                return Enumerable.Range(1, 36).Where(i => i % 2 == 0).ToList();

            case Choice.Odd:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра нечетните числа"));
                return Enumerable.Range(1, 36).Where(i => i % 2 == 1).ToList();

            case Choice.OneToTwelve:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра числата от 1 до 12."));
                return Enumerable.Range(1, 12).ToList();

            case Choice.ThirteenToTwentyFour:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра числата от 13 до 24."));
                return Enumerable.Range(13, 12).ToList();

            case Choice.TwentyfiveToThirtySix:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра числата от 25 до 36."));
                return Enumerable.Range(25, 12).ToList();

            case Choice.FirstHalf:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра числата от 1 до 18"));
                return Enumerable.Range(1, 18).ToList();

            case Choice.SecondHalf:
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Ти избра числата от 19 до 36."));
                return Enumerable.Range(19, 18).ToList();

            default:
                return null;
        }
    }

    private static async Task<InteractivityResult<DiscordMessage>> GetMessageAsync(InteractionContext ctx, char[] separators, int numbers)
    {
        if (_userChoices is null)
        {
            _userChoices = new List<int>();
        }
        else
        {
            _userChoices.Clear();
        }
        return await ctx.Channel.GetNextMessageAsync(m =>
        {
            var nums = m.Content.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (nums.Length == numbers)
            {
                foreach (var item in nums)
                {
                    var real = int.TryParse(item, out int num);
                    if (real)
                    {
                        _userChoices.Add(num);
                    }
                    else
                    {
                        ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .WithContent("Не намерих валидно число, напиши отново."));
                        _userChoices.Clear();
                        return false;
                    }
                }
            }
            else
            {
                _userChoices.Clear();
                return false;
            }
            return m.Author == ctx.Member && m.Content.Split(' ').Select(i => int.TryParse(i, out _)).All(b => b == true);
        }, TimeSpan.FromSeconds(5));
    }
}
