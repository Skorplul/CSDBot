using Discord.WebSocket;
using PRMainBot.API;
using Log = PRMainBot.API.Log;

namespace PRMainBot.Commands.InDiscord;

public static class maintenance
{
    public static async Task Execute(SocketSlashCommand command)
    {
        await command.RespondAsync("Wartungsarbeit gesetzt...");

        if (!Settings.IsMaintenance)
        {
            Log.Debug("Activating Maintenance...");
            Settings.IsMaintenance = true;
        }
        else
        {
            Log.Debug("Deactivating Maintenance...");
            Settings.IsMaintenance = false;
        }
    }
}