using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HtmlAgilityPack;
using Presli.Classes;

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
            response += $"{number} ";
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent(response));
    }
    [SlashCommand("random_yugioh_karta", "Random Yu-Gi-Oh karta")]
    public async Task randomCard(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddFile(randomYuGiOhCard.RandomCard()));
    }
    [SlashCommand("random_gif", "Произволен gif с мен")]
    public async Task ScrapeGif(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync (InteractionResponseType.DeferredChannelMessageWithSource);

        string url = "https://tenor.com/search/aethelthryth-gifs";
        HttpClient client = new HttpClient();
        string html = client.GetStringAsync(url).Result;
        HtmlDocument tenor = new HtmlDocument();

        tenor.LoadHtml(html);
        var gif = tenor.DocumentNode.Descendants("img")
            .Where(img => img.GetAttributeValue("alt", "")
            .Contains("Aethelthryth"))
            .Select(node => node.GetAttributeValue("src", ""))
            .ToList();

        var number = Random.Shared.Next(gif.Count);
        var response = gif[number];

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent($"{response}"));
    }
    [SlashCommand("send_direct_msg", "Произволен текст на юзера")]
    public async Task SendDirectMsg(InteractionContext ctx,[Option("съобщение", "Съобщение което да ти изпратя на лично")] string msg)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        await ctx.Member.SendMessageAsync(msg);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("Пратих ти ;)"));

        //ctx.Client.MessageCreated += async (s, e) =>
        //{
        //    if (!e.Message.Author.IsBot)
        //    {
        //        await ctx.Member.SendMessageAsync(e.Message.Content);
        //    }
        //};
    }
}