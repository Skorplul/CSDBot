using Discord.WebSocket;
using Discord.Interactions;
using Discord;
using Log = PRMainBot.API.Log;

namespace PRMainBot.Commands;

public class Test
{
    [SlashCommand("test", "test something")]
    [RequireUserPermission(GuildPermission.ViewAuditLog)]
    async public static Task Execute(SocketSlashCommand command)
    {
        Log.Debug($"Command \"{command.Data.Name}\" has been executed by {command.User.GlobalName}!");
        await command.RespondAsync($"You executed {command.Data.Name}");
    }
}
