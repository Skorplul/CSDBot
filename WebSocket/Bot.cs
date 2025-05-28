using System.Text.Json;
using PRMainBot.Commands;
using PRMainBot.Commands.InDiscord;
using Discord;
using Discord.WebSocket;
using Log = PRMainBot.API.Log;
using PRMainBot.API;

#nullable enable

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
            if (commandName == "maintenace")
            {
                await maintenance.Execute(command);
            }

            if (commandName == "stop")
            {
                if (command.User.Id == 504875989776596992)
                {
                    Log.Debug($"Command {command.Data.Name} has been executed by {command.User.GlobalName}!");
                    await command.RespondAsync($"You stopped the bot!", ephemeral: true);
                    await WebSocket._client.StopAsync();
                    await WebSocket._client.LogoutAsync();
                    Environment.Exit(0);
                }
                else
                {
                    await command.RespondAsync("No Permission", ephemeral: true);
                }
            }

            if (commandName == "reload")
            {
                if (command.User.Id == 504875989776596992)
                {
                    Log.Debug($"Commands have been reloaded by {command.User.GlobalName}!");

                    await reload.Execute(command);
                }
                else
                {
                    await command.RespondAsync("No Permission", ephemeral: true);
                }

            }
        }

        private static int? _count;
        internal static async Task Counting(SocketMessage msg)
        {
            Log.Debug($"Recieved message.");
            if (msg.Channel.Id != Config.Instance.CountingChannel)
                return;

            if (!int.TryParse(msg.Content, out int msgNr))
                return;

            if (_count == null)
            {
                _count = Config.Instance.LastCountNr;
            }
            
            if (msgNr == _count++)
            {
                _count++;
                await msg.AddReactionAsync(Emote.Parse(":white_check_mark:"));
                return;
            }
            else
            {
                await msg.AddReactionAsync(Emote.Parse("<:redcross:758380151238033419>"));
                _count = 0;
                await msg.Channel.SendMessageAsync($"Was ein Skillissue. Die richtige Nummer wäre {_count++} gewesen.\nEs geht wieder bei 1 los!");
                return;
            }
        }

        private static bool _presenceIsRunning = false;
        internal static async Task PlayerPresence()
        {
            if (_presenceIsRunning)
            {
                Log.Warn("PlayerPresence already running — ignoring this call.");
                return;
            }

            _presenceIsRunning = true;

            try
            {
                await WebSocket._client.SetActivityAsync(new Game("Bot mit Skill", ActivityType.Playing));
                _presenceIsRunning = false;
                return;
            }
            catch (Exception ex)
            {
                Log.Error($"Error: {ex.Message}");
                await Task.Delay(30000);
            }
        }
    }
}
