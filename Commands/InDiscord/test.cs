using Discord.WebSocket;
using Log = CSDBot.API.Log;

namespace CSDBot.Commands;

public class Test
{
    async public static void Execute(SocketSlashCommand command)
    {
        Log.Debug($"Command \"{command.Data.Name}\" has been executed by {command.User.GlobalName}!");
        await command.RespondAsync($"You executed {command.Data.Name}");
    }
}
