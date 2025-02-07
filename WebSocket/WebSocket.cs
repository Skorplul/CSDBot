using CSDBot.Commands.InConsole;
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

            await _client.LoginAsync(TokenType.Bot, bottoken);
            await _client.StartAsync();

            await Task.Run(() => HandleConsoleInput());

            await Task.Delay(-1);
        }

        // This method runs on a background thread and processes console commands.
        private static void HandleConsoleInput()
        {
            while (true)
            {
                string? input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Exit.Execute();
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
            }
        }
    }
}
