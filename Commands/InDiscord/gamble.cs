using System.Threading.Tasks;
using Discord.WebSocket;
using Log = CSDBot.API.Log;

namespace CSDBot.Commands.InDiscord;

public static class Gamble
{
    private static Random random = new Random();

    private static int DeathInt = 3;

    public static async Task Execute(SocketSlashCommand command)
    {
        int LuckGen = random.Next(0,6);

        if (LuckGen == DeathInt)
        {
            var user = (SocketGuildUser)command.User;

            await user.ModifyAsync(x => x.TimedOutUntil = DateTimeOffset.UtcNow.AddMinutes(10));
            await command.RespondAsync("Das war wohl nichts. You Dead :)");
        }
        else
        {
            await command.RespondAsync("Glück gehabt!");
        }
    }
}
