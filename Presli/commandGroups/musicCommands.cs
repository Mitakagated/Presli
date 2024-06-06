using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players;
using Lavalink4NET.Tracks;
using Presli.Classes;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;
using DSharpPlus.SlashCommands.Attributes;

namespace Presli.commandGroups;

public class musicCommands : ApplicationCommandModule
{
    public IAudioService _audioService { get; set; }
    private readonly IOptions<QueuedLavalinkPlayerOptions> _options = (IOptions<QueuedLavalinkPlayerOptions>) new QueuedLavalinkPlayerOptions { Label = "Presli" };

    readonly SongsQueue songs = SongsQueue.Instance;

    public musicCommands(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public enum TypeSearches
    {
        Search,
        Link
    }

    //[SlashCommand("vlizai", "shte vlqza v talk (ne moga da govorq)")]
    //public async Task Join(InteractionContext ctx)
    //{
    //    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
    //    var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false).ConfigureAwait(false);

    //    if (player == null)
    //    {
    //        return;
    //    }

    //    if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
    //    {
    //        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
    //                .WithContent("pyrvo voice channel, sled tova shte prisystvam"));
    //        return;
    //    }

    //    await ctx.Member.VoiceState.Channel.ConnectAsync(node);
    //    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
    //            .WithContent($"vytre sym v {ctx.Member.VoiceState.Channel}"));
    //}

    [SlashCommand("NAPUSNI", "shte napusna bezceremonno")]
    public async Task Leave(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false).ConfigureAwait(false);

        if (player == null)
        {
            return;
        }

        await player.DisconnectAsync().ConfigureAwait(false);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("НАПУСКАМ"));
    }
    [SlashCommand("puskai", "shte probvam da sym DJ :sunglasses:")]
    public async Task Play(InteractionContext ctx, [Option("РежимНаТърсене", "Избор между нормално търсене, или с линк")] TypeSearches searchType, [Option("Search", "Полето за търсене")] string search)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: true).ConfigureAwait(false);

        if (player == null)
        {
            return;
        }

        var track = await _audioService.Tracks
        .LoadTrackAsync(search, TrackSearchMode.YouTube)
        .ConfigureAwait(false);

        if (track is null)
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder
            {
                Content = "tyrseneto za {search} napravi abort"
            }).ConfigureAwait(false);
            return;
        }

        await player.PlayAsync(track).ConfigureAwait(false);
        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder
        {
            Content = $"ЯША ВЕ {track.Title} :sunglasses:"
        }).ConfigureAwait(false);

        //if (loadResult.LoadResultType == LavalinkLoadResultType.TrackLoaded)
        //{
        //    var track = loadResult.Tracks.First();
        //    songs.EnqueueSong(track);
        //}
        //else if (loadResult.LoadResultType == LavalinkLoadResultType.PlaylistLoaded)
        //{
        //    var track = loadResult.Tracks;
        //    songs.EnqueueSong(track);
        //}

        //if (conn.CurrentState.CurrentTrack == null)
        //{
        //    player = songs.DequeueSong();
        //    await conn.PlayAsync(player);
        //    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
        //        .WithContent($"ЯША ВЕ {player.Title} :sunglasses:"));
        //}
        //else
        //{
        //    string condition;
        //    if (loadResult.Tracks.Count() > 1 || loadResult.Tracks.Any())
        //    {
        //        condition = "песни";
        //    }
        //    else
        //    {
        //        condition = "песен";
        //    }

        //    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
        //        .WithContent($"Заредих {loadResult.Tracks.Count()} {condition}"));
        //}
        //conn.PlaybackFinished += async (sender, e) =>
        //{
        //    if (conn.CurrentState.CurrentTrack != null)
        //    {
        //        await ctx.Channel.SendMessageAsync($"ЯША ВЕ {conn.CurrentState.CurrentTrack.Title} :sunglasses:");
        //    }
        //    else
        //    {
        //        await ctx.Channel.SendMessageAsync($"Дотук с траковете");
        //    }
        //};
    }

    [SlashCommand("pauzirai", "pochivka? nqma problem")]
    public async Task Pause(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false);

        if (player is null)
        {
            return;
        }

        if (player.State is PlayerState.Paused)
        {
            await ctx.CreateResponseAsync("Нямам какво да паузирам :XD:").ConfigureAwait(false);
            return;
        }

        await player.PauseAsync().ConfigureAwait(false);
        await ctx.CreateResponseAsync("Паузирано womp womp").ConfigureAwait(false);
    }

    [SlashCommand("prodylji", "Пускам откъдето стигнахме.")]
    public async Task Resume(InteractionContext ctx)
    {
        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false);

        if (player is null)
        {
            return;
        }

        if (player.State is not PlayerState.Paused)
        {
            await ctx.CreateResponseAsync("Плейъра не е паузиран.").ConfigureAwait(false);
            return;
        }

        await player.ResumeAsync().ConfigureAwait(false);
        await ctx.CreateResponseAsync("Продължавам :sunglasses:").ConfigureAwait(false);
    }

    [SlashCommand("toggleloop", "tekushtata pesen da se povtarq")]
    public async Task LoopToggle(InteractionContext ctx)
    {
        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false);

        if (player is null)
        { 
            return;
        }

        if (player.RepeatMode is TrackRepeatMode.None)
        {
            player.RepeatMode = TrackRepeatMode.Track;
            await ctx.CreateResponseAsync("Loopa е пуснат.");
        }
        else if (player.RepeatMode is TrackRepeatMode.Track)
        {
            player.RepeatMode = TrackRepeatMode.None;
            await ctx.CreateResponseAsync("Loopa е спрян.");
        }
    }

    [SlashCommand("skip", "Пропуска текущата песен, и пуска следваща ако има")]
    public async Task Skip(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false);

        if (player is null)
        {
            return;
        }

        if (player.CurrentTrack is null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder() { Content = "Няма какво да се skipne :XD:" }).ConfigureAwait(false);
            return;
        }

        await player.SkipAsync().ConfigureAwait(false);

        var track = player.CurrentTrack;

        if (track is not null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder() { Content = $"Продължаваме с {track.Uri}" }).ConfigureAwait(false);
        }
        else
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder() { Content = "Понеже няма какво да пусна, сега ще е тихо :)" }).ConfigureAwait(false);
        }
    }

    [SlashCommand("stop", "Спира музиката, без да напускам.")]
    public async Task Stop(InteractionContext ctx)
    {
        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false);

        if (player is null)
        {
            return;
        }

        if (player.CurrentTrack is null)
        {
            await ctx.CreateResponseAsync("Нищо не е пуснато.").ConfigureAwait(false);
            return;
        }

        await player.StopAsync().ConfigureAwait(false);
        await ctx.CreateResponseAsync("Спрях.").ConfigureAwait(false);
    }

    [SlashCommand("showsongs", "Показва всички песни")]
    public async Task ShowSongs(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false);

        if (player is null)
        {
            return;
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.White)
                        .WithTitle("Брой песни:")
                        .WithDescription($"{player.Queue.Count}")
                        .WithFooter("PresliTheBest56")
                        .Build())
            .WithTTS(true));
    }

    [SlashCommand("nepipai", "not a command")]
    [SlashRequireOwner]
    public async Task<QueuedLavalinkPlayer?> GetPlayerAsync(InteractionContext ctx,[Option("nqma_takova", "pi6ok")] bool connectToVoiceChannel = true)
    {
        var channelBehavior = connectToVoiceChannel
            ? PlayerChannelBehavior.Join
            : PlayerChannelBehavior.None;

        var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);

        var result = await _audioService.Players
            .RetrieveAsync(ctx.Guild.Id, ctx.Member.VoiceState.Channel.Id, playerFactory: PlayerFactory.Queued, _options, retrieveOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "Първо voice channel, след това ще присъствам.",
                PlayerRetrieveStatus.BotNotConnected => "Няма връзка със сателита.",
                _ => "ИСКАМ ДА МРЪДНА",
            };

            await ctx.CreateResponseAsync(errorMessage).ConfigureAwait(false);
            return null;
        }

        return result.Player;
    }
}