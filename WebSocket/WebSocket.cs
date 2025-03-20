using System.Reflection;
using Discord.Interactions;
using PRMainBot.Commands.InConsole;
using Discord;
using Discord.WebSocket;
using Log = PRMainBot.API.Log;
using PRMainBot.Commands;
using Discord.Net;
using Newtonsoft.Json;

namespace PRMainBot
{
    public class WebSocket
    {
        public static DiscordSocketClient? _client;

        public static async Task Main()
        {
            string bottoken = Config.Instance.BotToken;
            Log.Debug("Token Loaded...");
            await Task.Delay(100);

            _client = new DiscordSocketClient();

            _client.Log += Log.Loging;
            _client.SlashCommandExecuted += Bot.SlashCommandHandler;

            await _client.LoginAsync(TokenType.Bot, bottoken);
            await _client.StartAsync();
            _client.Ready += async () =>
            {
                await ReadyAsync();

                Log.Debug("Bot is ready! Setting presence...");
                
                // Start presence update in the background
                _ = Task.Run(Bot.UpdatePrecence);
            };

            await Task.Run(() => HandleConsoleInput());

            await Task.Delay(-1);
        }

        private static async Task ReadyAsync()
        {
            var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.Namespace == "PRMainBot.Commands");

            foreach (var type in commandTypes)
            {
                var methods = type.GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(SlashCommandAttribute), false).Length > 0);

                foreach (var method in methods)
                {
                    var attribute = (SlashCommandAttribute)method.GetCustomAttribute(typeof(SlashCommandAttribute));
                    var commandBuilder = new SlashCommandBuilder()
                        .WithName(attribute.Name)
                        .WithDescription(attribute.Description);

                    try
                    {
                        await _client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
                        Log.Debug($"Command {attribute.Name} registered successfully.");
                    }
                    catch (HttpException exception)
                    {
                        var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                        Log.Error(json);
                    }
                }
            }
        }


        // This method runs on a background thread and processes console commands.
        private static async Task HandleConsoleInput()
        {
            while (true)
            {
                string? input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    Log.Command($">>> {input}");
                    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Exit.Execute();
                    }
                    else if (input.Equals("maintenance", StringComparison.OrdinalIgnoreCase))
                    {
                        Maintenance.Execute();
                    }
                    else
                    {
                        Log.Command($"Command {input} does not exist!");
                    }


                    
                    // else if (input.Equals("", StringComparison.OrdinalIgnoreCase))
                    // {
                    //    
                    // }
                    // else
                    // {
                    //      Log.Debug($"Command {input} has been registered.")
                    // }
                }

                await Task.Delay(100);
            }
        }
    }
}
