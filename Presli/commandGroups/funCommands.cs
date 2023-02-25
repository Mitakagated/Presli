using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Presli.commandGroups;

public class funCommands : ApplicationCommandModule
{
    [SlashCommand("ping", "probvai")]
    public async Task Ping(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent("fuck you"));
    }

    [SlashCommand("random_9_cifri", "generirane na 9 chisla ot 0 do 9")]
    public async Task randomNum(InteractionContext ctx)
    {
        var num = new Random();
        int[] nums = new int[9];
        var response = "";
        for (var i = 0; i < nums.Length; i++)
        {
            nums[i] = num.Next(0, 10);
        }

        foreach (var number in nums)
        {
            response += number + " ";
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent(response));
    }
}