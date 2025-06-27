using Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PRMainBot.API;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PRMainBot.Database
{
    internal static class API
    {
        private static class PlayerDataCache
        {
            internal static readonly ConcurrentDictionary<string, PlayerData> Data = new();

            public static PlayerData Get(string id)
                => Data.TryGetValue(id, out var result) ? result : null;

            public static void Set(string id, PlayerData playerData)
                => Data[id] = playerData;
        }

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

        /// Tokens for <see cref="VerifiedUpdate(SocketGuild)">
        private static bool VerifyUpdateCancled = false;
        private static bool VerifyUpdateRunning = false;

        /// Tokens for <see cref="UpdateRanks(SocketGuild)">
        private static bool UpdateRanksCancled = false;
        private static bool UpdateRanksRunning = false;

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
            VerifyUpdateCancled = true;
            UpdateRanksCancled = true;
            _collection = null;
            DbLoaded = false;
        }

        public static async Task<PlayerData> GetUserData(this SocketUser user)
        {
            return await _collection.Find(u => u.DiscordId == user.Id.ToString() + "@discord").FirstOrDefaultAsync();
        }

        /// <summary>
        /// Loads all MongoDB documents into <see cref="PlayerDataCache.Data">.
        /// </summary>
        private static void LoadAllDataFromDatabase()
        {
            Task.Run(async () =>
            {
                var allPlayers = await _collection.Find(_ => true).ToListAsync();
                foreach (var player in allPlayers)
                {
                    try
                    {
                        PlayerDataCache.Data[player.Id] = player;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Player with wrong atributes, skipping...");
                        continue;
                    }
                }

                Log.Debug($"Loaded {PlayerDataCache.Data.Count} player data entries into memory.");
            }).GetAwaiter().GetResult(); // Block here if you must to ensure data is ready.
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
        /// Seting the verified Role for every verified user.
        /// </summary>
        /// <param name="guild">The <see cref="SocketGuild"/> to go through.</param>
        /// <returns>The Task for this operation</returns>
        internal static async Task VerifiedUpdate(SocketGuild guild)
        {
            if (VerifyUpdateRunning)
                return;

            VerifyUpdateRunning = true;

            while (!VerifyUpdateCancled)
            {
                LoadAllDataFromDatabase();
                var datacache = PlayerDataCache.Data;
                PlayerDataCache.Data.Clear();

                foreach (var user in datacache.Values)
                {
                    if (!user.Verified)
                        continue;

                    if (string.IsNullOrWhiteSpace(user.DiscordId))
                        continue;

                    string idDigits = new string(user.DiscordId.Where(char.IsDigit).ToArray());

                    if (!ulong.TryParse(idDigits, out ulong dcid))
                    {
                        Log.Info("Skipping user (couldn't parse Discord ID)");
                        continue;
                    }

                    var dcuser = await WebSocket._client.Rest.GetGuildUserAsync(guild.Id, dcid);

                    var veriRole = guild.Roles.FirstOrDefault(r => r.Id == (ulong)Enums.Roles.Verified);
                    if (veriRole == null)
                    {
                        Log.Warn("Verification role not found.");
                        break; // Stop loop; no point continuing if role doesn't exist
                    }

                    if (dcuser.RoleIds.Contains(veriRole.Id))
                        continue;

                    Log.Info($"Adding Role to {dcuser.Username}");
                    await dcuser.AddRoleAsync(veriRole);
                }
                datacache = null;

                await Task.Delay(30000);
            }
            VerifyUpdateRunning = false;
        }

        /// <summary>
        /// Update Roles for SCP:SL server.
        /// </summary>
        /// <param name="guild">The <see cref="SocketGuild"/> to go through.</param>
        /// <returns>The Task for this operation</returns>
        internal static async Task UpdateRanks(SocketGuild guild)
        {
            if (UpdateRanksRunning)
                return;
            UpdateRanksRunning = true;

            while (!UpdateRanksCancled)
            {
                LoadAllDataFromDatabase();
                var datacache = PlayerDataCache.Data;
                PlayerDataCache.Data.Clear();

                foreach (PlayerData user in datacache.Values)
                {
                    if (!user.Verified)
                        continue;

                    string idDigits = new string(user.DiscordId.Where(char.IsDigit).ToArray());

                    if (!ulong.TryParse(idDigits, out ulong dcid))
                    {
                        Log.Info("Skipping user (couldn't parse Discord ID)");
                        continue;
                    }

                    var dcuser = await WebSocket._client.Rest.GetGuildUserAsync(guild.Id, dcid);

                    if (dcuser.RoleIds.Any(id => Enums.RoleLookup.ContainsKey(id)))
                    {
                        var matchedRoles = dcuser.RoleIds
                            .Where(id => Enums.RoleLookup.ContainsKey(id))
                            .Select(id => Enums.RoleLookup[id])
                            .ToList();

                        if (matchedRoles.FirstOrDefault() == Enums.Roles.Verified)
                            matchedRoles.Remove(matchedRoles.FirstOrDefault());

                        user.Rank = matchedRoles.FirstOrDefault().ToRoleString() ?? "Keine Rolle";

                        var filter = Builders<PlayerData>.Filter.Eq(p => p.Id, user.Id);

                        var update = Builders<PlayerData>.Update
                            .Set(p => p.Rank, user.Rank);

                        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
                    }
                }

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
                if (!string.IsNullOrEmpty(player.VerificationToken) && string.IsNullOrEmpty(player.DiscordId))
                    return (3, null); // Error 3: User already started verification

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
