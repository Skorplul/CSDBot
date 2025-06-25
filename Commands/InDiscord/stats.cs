using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using PRMainBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMainBot.Commands.InDiscord
{
    public static class stats
    {
        async public static Task Execute(SocketSlashCommand command)
        {
            var _userStats = await command.User.GetUserData();

            if (_userStats == null)
            {
                await command.RespondAsync("Du bist noch nicht verifiziert, bitte führe /verify aus um diesen Command benutzen zu können.");
                return;
            }
            var ts = TimeSpan.FromSeconds(_userStats.Playtime);

            string pt = String.Format("{0} Stunden, {1} Minuten, {2} Sekunden", (int)ts.TotalHours, ts.Minutes, ts.Seconds);

            var _killsField = new EmbedFieldBuilder()
                .WithName("Kills")
                .WithValue($"{_userStats.Kills}");

            var _deathsField = new EmbedFieldBuilder()
                .WithName("Tode")
                .WithValue($"{_userStats.Deaths}");

            var _currentXpField = new EmbedFieldBuilder()
                .WithName("Aktuelle XP")
                .WithValue($"{_userStats.XP}");

            var _levelField = new EmbedFieldBuilder()
                .WithName("Level")
                .WithValue($"{_userStats.Level}");

            var _playtimeField = new EmbedFieldBuilder()
                .WithName("Spielzeit")
                .WithValue($"{pt}");

            var _reaconsField = new EmbedFieldBuilder()
                .WithName("Recoins")
                .WithValue($"{_userStats.Recons}");

            var _footer = new EmbedFooterBuilder()
                .WithText($"Made By @skorp1.0 • {DateTime.Now}")
                .WithIconUrl("https://cdn.discordapp.com/avatars/504875989776596992/8542a836150144bf7db92fea8f7a886c.png?size=1024");

            var _embed = new EmbedBuilder()
                .WithColor(new Color(45, 45, 255))
                .WithAuthor($"@{command.User.GlobalName}")
                .WithFields(_killsField, _deathsField, _currentXpField, _levelField, _playtimeField, _reaconsField)
                .WithFooter(_footer);
        }
    }
}
