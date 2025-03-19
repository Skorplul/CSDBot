using System.Text.Json;
using PRMainBot.Commands;
using PRMainBot.Commands.InDiscord;
using Discord;
using Discord.WebSocket;
using Log = PRMainBot.API.Log;
using PRMainBot.API;

namespace PRMainBot
{
    internal class Bot
    {
        public static async Task UpdatePrecence()
        {
            while(true)
            {
                Log.Debug("Updating Precence");
                await PlayerPresence();
                await Task.Delay(500);
            }
        }

        public static async Task SlashCommandHandler(SocketSlashCommand command)
        {
             string commandName = command.Data.Name;

            if (commandName == "test")
            {
                await Test.Execute(command);
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
        }

        internal static async Task PlayerPresence()
        {
            string apiUrl = $"https://api.scpslgame.com/serverinfo.php?id={Config.Instance.SL_Acc_ID}&key={Config.Instance.SL_API_Key}&players=true&online=true";

            try
            {
                using HttpClient client = new HttpClient();
                string responseJson = await client.GetStringAsync(apiUrl);

                // Parse JSON response
                var responseObject = JsonSerializer.Deserialize<ApiResponse>(responseJson);

                if (responseObject != null && responseObject.Success && responseObject.Servers.Length > 0)
                {
                    var server = responseObject.Servers[0]; // the first server in the list

                    if (!server.Online)
                    {
                        await WebSocket._client.SetActivityAsync(new Game($"WARTUNGSARBEITEN", ActivityType.Playing));
                        await WebSocket._client.SetStatusAsync(UserStatus.DoNotDisturb);

                        await Task.Delay(1000*responseObject.Cooldown + 1000*5);
                    }
                    else if (!server.Online)
                    {
                        await WebSocket._client.SetActivityAsync(new Game($"Offline", ActivityType.Playing));
                        await WebSocket._client.SetStatusAsync(UserStatus.DoNotDisturb);

                        await Task.Delay(1000*responseObject.Cooldown + 1000*5);
                    }
                    else
                    {
                        await WebSocket._client.SetActivityAsync(new Game($"{server.Players} Online", ActivityType.Playing));
                        if (server.Players.Contains("0/"))
                        await WebSocket._client.SetStatusAsync(UserStatus.Idle);

                        await Task.Delay(1000*responseObject.Cooldown + 1000*5);
                    }
                }
                else
                {
                    Log.Warn("No servers found or API returned an error.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error: {ex.Message}");
            }
        }
    }

    // Classes for JSON deserialization
    public class ApiResponse
    {
        public bool Success { get; set; }
        public int Cooldown { get; set; }
        public ServerInfo[] Servers { get; set; }
    }

    public class ServerInfo
    {
        public int ID { get; set; }
        public int Port { get; set; }
        public bool Online { get; set; }
        public string Players { get; set; } // Example format: "0/20"
    }
}
