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
    public PlayerHelper PlayerHelper {  get; set; }
    public IAudioService audioService { get; set; }
    private TrackSearchMode[] trackSearchModes = { TrackSearchMode.YouTube, TrackSearchMode.YouTubeMusic, TrackSearchMode.YandexMusic, TrackSearchMode.AppleMusic, TrackSearchMode.Deezer, TrackSearchMode.SoundCloud, TrackSearchMode.Spotify, TrackSearchMode.None };

    public enum SearchModes
    {
        Youtube = 0,
        YoutubeMusic = 1,
        YandexMusic = 2,
        AppleMusic = 3,
        Deezer = 4,
        SoundCloud = 5,
        Spotify = 6,
        None = 7
    }

    [SlashCommand("vlizai", "shte vlqza v talk (ne moga da govorq)")]
    public async Task Join(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: true).ConfigureAwait(false);

        if (player == null)
        {
            return;
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"vytre sym v {ctx.Member.VoiceState.Channel}"));
    }

    [SlashCommand("NAPUSNI", "shte napusna bezceremonno")]
    public async Task Leave(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: false).ConfigureAwait(false);

        if (player == null)
        {
            return;
        }

        await player.DisconnectAsync().ConfigureAwait(false);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("НАПУСКАМ"));
    }
    [SlashCommand("puskai", "shte probvam da sym DJ :sunglasses:")]
    public async Task Play(InteractionContext ctx, [Option("Search", "Полето за търсене")] string search, [Option("Provider", "Мястото, откъдето ще търся за музика")] SearchModes searchMode)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: true).ConfigureAwait(false);

        if (player == null)
        {
            return;
        }

        var track = await audioService.Tracks
        .LoadTrackAsync(search, trackSearchModes[(int)searchMode])
        .ConfigureAwait(false);

        if (track is null)
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder
            {
                Content = $"tyrseneto za {search} napravi abort"
            }).ConfigureAwait(false);
            return;
        }

        await player.PlayAsync(track).ConfigureAwait(false);
        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder
        {
            Content = $"ЯША ВЕ {track.Title} :sunglasses:"
        }).ConfigureAwait(false);
    }

    [SlashCommand("pauzirai", "pochivka? nqma problem")]
    public async Task Pause(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: false);

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
        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: false);

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
        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: false);

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
        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: false);

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
        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: false);

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

        var player = await PlayerHelper.GetPlayerAsync(ctx, connectToVoiceChannel: false);

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
}