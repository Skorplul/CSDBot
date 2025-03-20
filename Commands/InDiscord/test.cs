using Discord.WebSocket;
using Log = PRMainBot.API.Log;

namespace PRMainBot.Commands;

public class Test
{
    async public static Task Execute(SocketSlashCommand command)
    {
        Log.Debug($"Command \"{command.Data.Name}\" has been executed by {command.User.GlobalName}!");
        await command.RespondAsync($"You executed {command.Data.Name}");
    }
}
