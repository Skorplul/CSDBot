using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using DotNetEnv;
using Log = CSDBot.API.Log;
using YamlDotNet.Serialization;
using System.IO;
using System.Reflection;
using YamlDotNet.Serialization.NamingConventions;
using Discord.Commands;

namespace CSDBot
{
    public static class WebSocket
    {
        public static DiscordSocketClient? _client;

        public static async Task Main()
        {
            string bottoken = Config.Instance.BotToken;
            Log.Debug("Token Loaded...");

            _client = new DiscordSocketClient();

            _client.Log += Log.Loging;
            _client.SlashCommandExecuted += SlashCommandHandler;

            // Use the loaded token to login
            await _client.LoginAsync(TokenType.Bot, bottoken);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

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
                    await _client.StopAsync();
                    await _client.LogoutAsync();
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
                    await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
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
