using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players;
using Lavalink4NET;
using Microsoft.Extensions.Options;

namespace Presli.Classes;
public class PlayerHelper
{
    private readonly IOptions<QueuedLavalinkPlayerOptions> _options;
    private readonly IAudioService _audioService;

    public PlayerHelper(IAudioService audioService, IOptions<QueuedLavalinkPlayerOptions> options)
    {
        _audioService = audioService;
        _options = options;
    }

    public async Task<QueuedLavalinkPlayer?> GetPlayerAsync(InteractionContext ctx, bool connectToVoiceChannel = true)
    {
        Console.WriteLine("GetPlayerAsync");
        var channelBehavior = connectToVoiceChannel
            ? PlayerChannelBehavior.Join
            : PlayerChannelBehavior.None;

        var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);

        var result = await _audioService.Players
            .RetrieveAsync(ctx.Guild.Id, ctx.Member.VoiceState.Channel.Id, playerFactory: PlayerFactory.Queued, _options, retrieveOptions)
            .ConfigureAwait(false);
        Console.WriteLine("Minat result");
        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "Първо voice channel, след това ще присъствам.",
                PlayerRetrieveStatus.BotNotConnected => "ИСКАМ ДА МРЪДНА",
                _ => "ИСКАМ ДА МРЪДНА",
            };
            Console.WriteLine(errorMessage);
            await ctx.CreateResponseAsync(errorMessage).ConfigureAwait(false);
            return null;
        }

        return result.Player;
    }
}
