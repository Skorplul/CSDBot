using Log = PRMainBot.API.Log;

namespace PRMainBot.Commands.InConsole;

public static class Exit
{
    public static void Execute()
    {
        Log.Debug("Shutting down...");
        // Closing DB
        Database.API.CloseDB();
        Database.API.VerifyUpdateCancled = true;

        // Shut down the Discord client gracefully.
        WebSocket._client?.LogoutAsync().GetAwaiter().GetResult();
        WebSocket._client?.StopAsync().GetAwaiter().GetResult();

        // Exit the application.
        Environment.Exit(0);
    }
}
