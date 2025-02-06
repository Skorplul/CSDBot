using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Log = CSDBot.API.Log;

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
            _client.SlashCommandExecuted += Bot.SlashCommandHandler;

            // Login and start the Discord bot
            await _client.LoginAsync(TokenType.Bot, bottoken);
            await _client.StartAsync();

            // Start a background task to process console input
            Task.Run(() => HandleConsoleInput());

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        // This method runs on a background thread and processes console commands.
        private static void HandleConsoleInput()
        {
            while (true)
            {
                // This call blocks until input is entered.
                string? input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    // Process the command
                    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Log.Debug("Shutting down...");

                        // Shut down the Discord client gracefully.
                        _client?.LogoutAsync().GetAwaiter().GetResult();
                        _client?.StopAsync().GetAwaiter().GetResult();

                        // Exit the application.
                        Environment.Exit(0);
                    }
                    else
                    {
                        // Handle other commands as needed
                        Log.Debug($"Received command: {input}");
                        // You can add more command handling logic here.
                    }
                }
            }
        }
    }
}
