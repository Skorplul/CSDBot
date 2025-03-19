using PRMainBot.API;
using Log = PRMainBot.API.Log;

namespace PRMainBot.Commands.InConsole;

public static class Maintenance
{
    public static void Execute()
    {
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