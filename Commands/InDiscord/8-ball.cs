using Discord.WebSocket;
using Log = CSDBot.API.Log;

namespace CSDBot.Commands;

public static class EightBall
{
    readonly static Random Random = new Random();

    async public static Task Execute(SocketSlashCommand command)
    {
        Log.Debug($"Command \"{command.Data.Name}\" has been executed by {command.User.GlobalName}!");
        
        string answer =  Config.Instance.BallAnswr[Random.Next(0, Config.Instance.BallAnswr.Count)];

        await command.RespondAsync($"{answer}");
    }
}
