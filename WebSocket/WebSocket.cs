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
        private static InteractionService? _commands;
        private static InteractionServiceConfig? _config;

        public static async Task Main()
        {
            string bottoken = Config.Instance.BotToken;
            Log.Debug("Token Loaded...");
            await Task.Delay(100);

            _client = new DiscordSocketClient();
            _config = new InteractionServiceConfig
            {
                LogLevel = LogSeverity.Info
            };
            _commands = new InteractionService(_client, _config);

            _client.Log += Log.Loging;
            _client.SlashCommandExecuted += Bot.SlashCommandHandler;
            _client.InteractionCreated += HandleInteraction;

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
            // Add modules to the InteractionService
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), null);

            // Register commands with Discord
            await _commands.RegisterCommandsGloballyAsync();
        }

        private static async Task HandleInteraction(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _commands.ExecuteCommandAsync(context, null);
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
                }

                await Task.Delay(100);
            }
        }
    }
}
