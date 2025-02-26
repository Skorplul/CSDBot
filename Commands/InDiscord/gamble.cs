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
        Log.Debug($"Command \"{command.Data.Name}\" has been executed by {command.User.GlobalName}!");
        
        int LuckGen = random.Next(0,6);

        if (LuckGen == DeathInt)
        {
            try
            {
                var user = (SocketGuildUser)command.User;

                await user.ModifyAsync(x => x.TimedOutUntil = DateTimeOffset.UtcNow.AddMinutes(10));
                await command.RespondAsync("Das war wohl nichts. You Dead :)");
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("50013: Missing Permissions"))
                {
                    await command.RespondAsync("Error: 50013 \n Bitte an @Skorp 1.0 melden!");
                    Log.Error(ex.ToString());
                }
            }
        }
        else
        {
            await command.RespondAsync("Glück gehabt!");
        }
    }
}
