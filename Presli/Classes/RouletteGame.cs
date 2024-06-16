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
    private static List<int>? _userChoices;
    private char[] separators = new char[] { ' ', ',' };
    public int pulledNum = 0;
    public bool foundNum;

    public enum Choice
    {
        [ChoiceName("Едно число")]
        One,
        [ChoiceName("Две числа")]
        Two,
        [ChoiceName("Три числа")]
        Three,
        [ChoiceName("Шест числа")]
        Six,
        [ChoiceName("Черни числа")]
        Black,
        [ChoiceName("Червени числа")]
        Red,
        [ChoiceName("Четни числа")]
        Even,
        [ChoiceName("Нечетни числа")]
        Odd,
        [ChoiceName("От 1 до 12")]
        OneToTwelve,
        [ChoiceName("От 13 до 24")]
        ThirteenToTwentyFour,
        [ChoiceName("От 25 до 36")]
        TwentyfiveToThirtySix,
        [ChoiceName("От 1 до 18")]
        FirstHalf,
        [ChoiceName("От 19 до 36")]
        SecondHalf
    }

    public async Task PullNumber(Choice choice, InteractionContext ctx)
    {
        var chosenNums = await PrepareList(choice, ctx);
        if (chosenNums is not null)
        {
            pulledNum = _nums[Random.Shared.Next(37) - 1];
            foundNum = chosenNums!.Contains(pulledNum);
        }
        else
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"Не написа валидни числа навреме. Опитай отново."));
            return;
        }    
    }

    private async Task<List<int>?> PrepareList(Choice choice, InteractionContext ctx)
    {
        //The selective number betting is wrong. I have no idea how to implement actual split, street, corner and 6-line betting, but this will do for now.
        switch (choice)
        {
            case Choice.One:
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Напиши едно число за рулетката. Пример: 21, до 36"));
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
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Напиши две числа за рулетката. Пример: 21, 15, до 36"));
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
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Напиши три числа за рулетката. Пример: 21, 15, 3, до 36"));
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
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Напиши шест числа за рулетката. Пример: 21, 15, 3, 6, 1, 29, до 36"));
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
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
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
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
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
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Ти избра четните числа."));
                return Enumerable.Range(1, 36).Where(i => i % 2 == 0).ToList();

            case Choice.Odd:
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Ти избра нечетните числа"));
                return Enumerable.Range(1, 36).Where(i => i % 2 == 1).ToList();

            case Choice.OneToTwelve:
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Ти избра числата от 1 до 12."));
                return Enumerable.Range(1, 12).ToList();

            case Choice.ThirteenToTwentyFour:
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Ти избра числата от 13 до 24."));
                return Enumerable.Range(13, 12).ToList();

            case Choice.TwentyfiveToThirtySix:
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Ти избра числата от 25 до 36."));
                return Enumerable.Range(25, 12).ToList();

            case Choice.FirstHalf:
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("Ти избра числата от 1 до 18"));
                return Enumerable.Range(1, 18).ToList();

            case Choice.SecondHalf:
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
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
                    if (real && num <= 36)
                    {
                        _userChoices.Add(num);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return m.Author == ctx.User;
            //return m.Author == ctx.Member && m.Content.Split(' ').Select(i => int.TryParse(i, out _)).All(b => b == true);
        }, TimeSpan.FromSeconds(25));
    }
}
