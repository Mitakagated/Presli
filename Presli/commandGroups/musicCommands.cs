using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace Presli.commandGroups;

public class musicCommands : ApplicationCommandModule
{
    [SlashCommand("vlizai", "shte vlqza v talk (ne moga da govorq)")]
    public async Task Join(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var lava = ctx.Client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("nqma vryzka sys satelita"));
            return;
        }

        var node = lava.ConnectedNodes.Values.First();
        if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("pyrvo voice channel, sled tova shte prisystvam"));
            return;
        }

        await ctx.Member.VoiceState.Channel.ConnectAsync(node);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"vytre sym v {ctx.Member.VoiceState.Channel}"));
    }

    [SlashCommand("NAPUSNI", "shte napusna bezceremonno")]
    public async Task Leave(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var lava = ctx.Client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("nqma vryzka sys satelita"));
            return;
        }

        var node = lava.ConnectedNodes.Values.First();

        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("nqma vryzka sys satelita"));
            return;
        }

        await conn.DisconnectAsync();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("NAPUSKAM"));
    }
    [SlashCommand("puskai", "shte probvam da sym DJ :sunglasses:")]
    public async Task Play(InteractionContext ctx, [Option("search", "tyrsi neshto idk")] string search)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var lava = ctx.Client.GetLavalink();
        // if (!lava.ConnectedNodes.Any())
        // {
        //     await ctx.EditResponseAsync(new DiscordWebhookBuilder()
        //             .WithContent("nqma vryzka sys satelita"));
        //     return;
        // }

        var node = lava.ConnectedNodes.Values.First();
        if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("pyrvo voice channel, sled tova DJ time :sunglasses:"));
            return;
        }
        await ctx.Member.VoiceState.Channel.ConnectAsync(node);
        
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("nqma vryzka sys satelita"));
            return;
        }

        var loadResult = await node.Rest.GetTracksAsync(search);

        if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed 
            || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent($"tyrseneto za {search} napravi abort"));
            return;
        }

        var track = loadResult.Tracks.First();

        await conn.PlayAsync(track);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"ЯША ВЕ {track.Title} :sunglasses:"));
    }

    [SlashCommand("pauzirai", "pochivka? nqma problem")]
    public async Task Pause(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("pyrvo voice channel, sled tova DJ time :sunglasses:"));
            return;
        }

        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("nqma vryzka sys satelita"));
            return;
        }
        if (conn.CurrentState.CurrentTrack == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("nqmam kakvo da pauziram :XD:"));
            return;
        }
        await conn.PauseAsync();
    }
    
    [SlashCommand("toggleloop", "tekushtata pesen da se povtarq")]
    public static async Task toggleloop(InteractionContext ctx,  [Choice("on", "true")]
        [Choice("off", "false")]
        [Option("toggle", "Дали да се повтаря или не")]bool toggle)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        // Start loop
            if (toggle == true)
            {
                if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .WithContent("pyrvo voice channel, sled tova DJ time :sunglasses:"));
                    return;
                }
                var lava = ctx.Client.GetLavalink();
                var node = lava.ConnectedNodes.Values.First();
                var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

                if (conn == null)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .WithContent("nqma vryzka sys satelita"));
                    return;
                }

                var length = Convert.ToInt32(conn.CurrentState.CurrentTrack.Length.TotalMilliseconds);
                if (conn.CurrentState.PlaybackPosition.Equals(conn.CurrentState.PlaybackPosition.TotalSeconds - 1))
                {
                    await conn.SeekAsync(TimeSpan.FromSeconds(0));
                }
            }
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("Loopa е пуснат"));
            if (toggle == false)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("Loopa е спрян"));
            }
            
            
    }
}