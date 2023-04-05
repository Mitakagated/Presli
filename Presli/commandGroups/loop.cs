using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace Presli.commandGroups;

public class loop
{
    public static async Task Loop(InteractionContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("pyrvo voice channel, sled tova DJ time :sunglasses:"));
            return;
        }
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("nqma vryzka sys satelita"));
            return;
        }

        var length = Convert.ToInt32(conn.CurrentState.CurrentTrack.Length.TotalMilliseconds);
        if (conn.CurrentState.PlaybackPosition.TotalMilliseconds == length)
        {
            await conn.SeekAsync(TimeSpan.FromSeconds(0));
        }
    }
}