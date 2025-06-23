using Discord;
using Discord.WebSocket;
using PRMainBot.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMainBot.Commands.InDiscord
{
    internal static class verification
    {
        async public static Task Execute(SocketSlashCommand command)
        {
            if (command.Data.Options.FirstOrDefault(arg => arg.Name == "steamid").ToString() == string.Empty || command.Data.Options.FirstOrDefault(arg => arg.Name == "steamid").ToString() == null)
                await command.RespondAsync("Du musst deine richtige SteamID64 Angeben. Website dafür -> https://steamid.xyz");

            var steamId = command.Data.Options.FirstOrDefault(arg => arg.Name == "steamid")?.Value?.ToString();

            if (steamId == null)
            {
                await command.RespondAsync("Da lief etwas schief, bitte gebt Skorp bescheid!");
                return;
            }
            else if (!steamId.Contains("@steam"))
            {
                steamId = steamId + "@steam";
            }

            var Link = Database.API.InitLink((SocketGuildUser)command.User, steamId).Result;

            if (Link.status == 0)
            {
                await command.User.SendMessageAsync($"Dein verifikations Token Lautet: {Link.message}\nGib diesen Token mit dem command `.verify <token>` in der Ö-Konsole auf dem Project Reload Server ein.");
                await command.RespondAsync("Dir wurde dein Token via DM geschickt, gib diesen nicht weiter!");
            }
            else if (Link.status == 1)
            {
                await command.RespondAsync("Du hast eine falsche SteamID angegeben. Bitte überprüfe nochmal deine SteamID, oder Frag Skorp.");
            }
            else if (Link.status == -1)
            {
                await command.RespondAsync("Es gabe einen Fehler beim Ausführen, bitte meldet das Skorp.");
            }
            
        }
    }
}
