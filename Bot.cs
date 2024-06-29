using Discord.WebSocket;

namespace JohnMaynard
{
    public class Bot
    {
        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            Console.WriteLine(command.CommandName);
            await command.RespondAsync($"You executed {command.Data.Name}");
        }
    }
}
