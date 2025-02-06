using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
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
                Log.Debug($"Command \"{command.Data.Name}\" has been executed by {command.User.GlobalName}!");
                //Console.WriteLine(command.CommandName);
                await command.RespondAsync($"You executed {command.Data.Name}");
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
                    await command.RespondAsync("No Permission");
                }
                return;
            }

            if (commandName == "reload")
            {
                // Let's do our global command
                var globalCommand = new SlashCommandBuilder();
                globalCommand.WithName("stop");
                globalCommand.WithDescription("Stop da bot!");

                try
                {
                    // With global commands we don't need the guild.
                    await WebSocket._client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                    // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                    // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
                }
                catch (HttpException exception)
                {
                    // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                    var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                    // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                    Log.Error(json);
                }
            }
        }
    }
}
