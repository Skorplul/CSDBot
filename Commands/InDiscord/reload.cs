using Discord;
using Discord.Net;
using Newtonsoft.Json;
using Log = CSDBot.API.Log;

namespace CSDBot.Commands.InDiscord;

public static class Reload
{
    async public static void Execute()
    {
        // Let's do our global command
                var globalCommand = new SlashCommandBuilder();

                int indexCount = 0;

                List<string> commName = new List<string>()
                {
                    "stop",
                    "8-ball",
                    "test",
                    "reload",
                };

                List<string> commDesc = new List<string>()
                {
                    "Stop da bot!",
                    "Ask him something!",
                    "test",
                    "reload commands?",
                };

                foreach (string comm in commName)
                { 
                    globalCommand.WithName(comm);
                    globalCommand.WithDescription(commDesc[indexCount]);
                    indexCount++;
                

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
