using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using Log = PRMainBot.API.Log;

namespace PRMainBot.Commands.InDiscord;

public static class reload
{
    async public static Task Execute(SocketSlashCommand command)
    {
        if (command != null)
            await command.RespondAsync("Reloading all commands now.", ephemeral: true);

        // Let's do our global command
        var globalCommand = new SlashCommandBuilder();

        int indexCount = 0;

        List<string> commName = new List<string>()
        {
            "stop",
            "test",
            "reload",
            "maintenace",
        };

        List<string> commDesc = new List<string>()
        {
            "Stop da bot!",
            "test",
            "reload commands?",
            "Setzt den Serverstatus auf Wartungsarbeiten.",
        };


        //need to redo for class itteration!!
        foreach (string comm in commName)
        { 
            globalCommand.WithName(comm);
            globalCommand.WithDescription(commDesc[indexCount]);
            
            switch (comm)
            {
                case "test":
                    globalCommand.WithDefaultMemberPermissions(GuildPermission.ViewAuditLog);
                    break;
                case "stop":
                    globalCommand.WithDefaultMemberPermissions(GuildPermission.Administrator);
                    break;
                case "reload":
                    globalCommand.WithDefaultMemberPermissions(GuildPermission.Administrator);
                    break;
                case "maintenace":
                    globalCommand.WithDefaultMemberPermissions(GuildPermission.ManageEvents);
                    break;
                default:
                    break;
            }

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
