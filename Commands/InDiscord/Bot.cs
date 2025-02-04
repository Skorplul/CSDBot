using Discord.WebSocket;

namespace CSDBot
{
    internal class Bot
    {
        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            Console.WriteLine(command.CommandName);
            await command.RespondAsync($"You executed {command.Data.Name}");
        }
    }
}
