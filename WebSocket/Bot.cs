using CSDBot.Commands;
using CSDBot.Commands.InDiscord;
using Discord.WebSocket;
using Log = CSDBot.API.Log;

namespace CSDBot
{
    internal class Bot
    {
        public static async Task SlashCommandHandler(SocketSlashCommand command)
        {
             string commandName = command.Data.Name;

            if (commandName == "test")
            {
                await Test.Execute(command);
            }

            if (commandName == "8-ball")
            {
                await EightBall.Execute(command);
            }

            if (commandName == "stop")
            {
                if (command.User.Id == 504875989776596992)
                {
                    Log.Debug($"Command {command.Data.Name} has been executed by {command.User.GlobalName}!");
                    await command.RespondAsync($"You stopped the bot!");
                    await WebSocket._client.StopAsync();
                    await WebSocket._client.LogoutAsync();
                    Environment.Exit(0);
                }
                else
                {
                    await command.RespondAsync("No Permission", null, false, true);
                }
            }

            if (commandName == "reload")
            {
                await Reload.Execute(command);
            }

            if (commandName == "gamble")
            {
                await Gamble.Execute(command);
            }
        }
    }
}
