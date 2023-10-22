using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Presli.Classes;
using PuppeteerSharp;

namespace Presli.commandGroups;

public class musicCommands : ApplicationCommandModule
{
    readonly SongsQueue songs = SongsQueue.Instance;
    LavalinkTrack? player;

    public enum TypeSearches
    {
        Search,
        Link
    }

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
    public async Task Play(InteractionContext ctx, [Option("РежимНаТърсене", "Избор между нормално търсене, или с линк")] TypeSearches searchType, [Option("Search", "Полето за търсене")] string search)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var lava = ctx.Client.GetLavalink();

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

        LavalinkLoadResult loadResult = new LavalinkLoadResult();

        if (searchType == TypeSearches.Search)
        {
            loadResult = await node.Rest.GetTracksAsync(search);
        }
        else if (searchType == TypeSearches.Link)
        {
            Uri uri = new Uri(search);
            loadResult = await node.Rest.GetTracksAsync(uri);
        }

        if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
            || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent($"tyrseneto za {search} napravi abort"));
            return;
        }

        if (loadResult.LoadResultType == LavalinkLoadResultType.TrackLoaded)
        {
            var track = loadResult.Tracks.First();
            songs.EnqueueSong(track);
        }
        else if (loadResult.LoadResultType == LavalinkLoadResultType.PlaylistLoaded)
        {
            var track = loadResult.Tracks;
            songs.EnqueueSong(track);
        }

        if (conn.CurrentState.CurrentTrack == null)
        {
            player = songs.DequeueSong();
            await conn.PlayAsync(player);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"ЯША ВЕ {player.Title} :sunglasses:"));
        }
        else
        {
            string condition;
            if (loadResult.Tracks.Count() > 1 || loadResult.Tracks.Any())
            {
                condition = "песни";
            }
            else
            {
                condition = "песен";
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"Заредих {loadResult.Tracks.Count()} {condition}"));
        }
        conn.PlaybackFinished += async (sender, e) =>
        {
            if (conn.CurrentState.CurrentTrack != null)
            {
                await ctx.Channel.SendMessageAsync($"ЯША ВЕ {conn.CurrentState.CurrentTrack.Title} :sunglasses:");
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"Дотук с траковете");
            }
        };
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
    public async Task LoopToggle(InteractionContext ctx)
    {
        songs.ToggleLoop();

        if (songs.loopEnabled)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("Loopa е пуснат"));
        }

        if (!songs.loopEnabled)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("Loopa е спрян"));
        }
    }

    [SlashCommand("skip", "Пропуска текущата песен, и пуска следваща ако има")]
    public async Task Skip(InteractionContext ctx)
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
                    .WithContent("Няма какво да се skipne :XD:"));
            return;
        }
        await conn.PlayAsync(songs.DequeueSong());
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent($"Продължаваме с {conn.CurrentState.CurrentTrack}"));
    }

    [SlashCommand("showsongs", "Показва всички песни")]
    public async Task ShowSongs(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.White)
                        .WithTitle("Брой песни:")
                        .WithDescription($"{songs.songQueue.Count}")
                        .WithFooter("PresliTheBest56")
                        .Build())
            .WithTTS(true));
    }
}