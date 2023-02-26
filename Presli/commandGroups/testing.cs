using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace Presli.commandGroups;

public class testing
{
    private bool IsLoopRunning = false;
    [SlashCommand("toggleloop", "tekushtata pesen da se povtarq")]
    public async Task ToggleLoop(InteractionContext ctx) 
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        // Check if loop is running
        if (IsLoopRunning) 
        {
            // Stop loop
            IsLoopRunning = false;
            Loop(ctx).Dispose();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("Няма loop"));
        }
        else
        {
            // Start loop
            IsLoopRunning = true;
            while (IsLoopRunning)
            {
                Loop(ctx);
            }
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("Loopa е пуснат"));
        }
    }
    
    public async Task Loop(InteractionContext ctx)
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
            conn.SeekAsync(TimeSpan.FromSeconds(0));
        }
    }
}