using System.Threading.Tasks;
using PRMainBot.Commands.InConsole;
using Discord;
using Discord.WebSocket;
using Log = PRMainBot.API.Log;
using PRMainBot.Commands.InDiscord;

namespace PRMainBot
{
    public static class WebSocket
    {
        internal static DiscordSocketClient? _client;

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
                Log.Debug("Bot is ready! Setting presence, reggistering commands and loading DB...");

                // Load DB
                Database.API.InitDB();
                _ = Database.API.VerifiedUpdate(_client.GetGuild(1329868400423338044));
                
                // Reggister commands
                reload.Execute(null);

                // Start presence update in the background
                _ = Task.Run(Bot.UpdatePrecence);
            };

            await Task.Run(() => HandleConsoleInput());

            await Task.Delay(-1);
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
