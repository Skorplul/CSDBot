using Log = CSDBot.API.Log;

namespace CSDBot.Commands.InConsole;

public static class Exit
{
    public static void Execute()
    {
        Log.Debug("Shutting down...");

        // Shut down the Discord client gracefully.
        WebSocket._client?.LogoutAsync().GetAwaiter().GetResult();
        WebSocket._client?.StopAsync().GetAwaiter().GetResult();

        // Exit the application.
        Environment.Exit(0);
    }
}
