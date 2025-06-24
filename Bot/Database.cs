using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PRMainBot.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PRMainBot.Database
{
    internal static class API
    {
        // Player data model
        public class PlayerData
        {
            [BsonId]
            public string Id { get; set; }
            public string DiscordId { get; set; } = null;
            public bool Verified { get; set; } = false;
            public string VerificationToken { get; set; }
            public string Nickname { get; set; }
            public string CustomNick { get; set; }
            public string Rank { get; set; }
            public bool NicknameChangable { get; set; } = true;
            public List<Warn> Warns { get; set; } = new List<Warn>();
            public List<Warn> Watchlists { get; set; } = new List<Warn>();
            public double Playtime { get; set; }
            public int XP { get; set; }
            public int RequiredXP { get; set; } = 230;
            public int Level { get; set; } = 1;
            public int Recons { get; set; }
            public int Kills { get; set; }
            public int Deaths { get; set; }
        }

        //Warn data structure
        public class Warn
        {
            public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
            public DateTime CreatedAt { get; set; }
            public string Reason { get; set; }
            public string Issuer { get; set; }
        }

        private static IMongoCollection<PlayerData>? _collection;
        internal static bool DbLoaded = false;

        internal static void InitDB()
        {
            if (DbLoaded) 
                return;

            string connectionString = $"mongodb://SL_MAIN:DuFetterNegger1+2@127.0.0.1:27017/?authSource=SL_MAIN";
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("SL_MAIN");
            _collection = db.GetCollection<PlayerData>("players");
            DbLoaded = true;
        }

        internal static void CloseDB()
        {
            _collection = null;
            DbLoaded = false;
        }

        /// <summary>
        /// Generate the Random Token for Verification.
        /// </summary>
        /// <param name="length">The length for the Token.</param>
        /// <returns>The Token.</returns>
        internal static string GenerateVerificationToken(int length = 25)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var result = new StringBuilder(length);
            foreach (byte b in data)
            {
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }


        /// <summary>
        /// For constant Rank updates. (only possible with OAuth2 flow for connections)
        /// </summary>
        /// <param name="guild">The <see cref="SocketGuild"/> to go through.</param>
        /// <returns>The Task for this opeeration</returns>w
        internal static async Task RankUpdate(SocketGuild guild)
        {
            await guild.DownloadUsersAsync();

            foreach (var user in guild.Users)
            {
                
            }
        }

        /// <summary>
        /// Linking a Discord Account to a SteamID64
        /// </summary>
        /// <param name="user">The <see cref="SocketGuildUser"/>, which executed and the account will be linked to.</param>
        /// <param name="steamId">The SteamID the user provided.</param>
        /// <returns>An <see cref="int"> "status" and a nullable <see cref="string"/> "message", which indicate the status of the operation and "message" returns the actuall verification Token on success.</returns>
        internal static async Task<(int status, string? message)> InitLink(SocketGuildUser user, string steamId)
        {
            var player = await _collection.Find(u => u.Id == steamId).FirstOrDefaultAsync();
            Log.Warn($"given steam: {steamId}\ngiven discord{user.Id}");

            if (player == null)
                return (1, null); // Error 1: "Not in DB" (prob. wrong SteamID)

            if (!player.Verified)
            {
                player.DiscordId = user.Id.ToString() + "@discord";
                player.VerificationToken = GenerateVerificationToken();

                var filter = Builders<PlayerData>.Filter.Eq(p => p.Id, player.Id);
                await _collection.ReplaceOneAsync(filter, player);
                return (0, player.VerificationToken); // Status 0: Success
            }
            else if (player.Verified)
                return (2, null); // Error 2: Player already verified

            return (-1, null); // Error -1: General Error
        }
    }
}
