using Discord.WebSocket;
using CSDBot.API;

namespace CSDBot
{
    internal class Bot
    {
        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            Log.Info($"@{command.User.GlobalName} executed {command.CommandName}");
            await command.RespondAsync($"You executed {command.Data.Name}");
        }
    }
}
