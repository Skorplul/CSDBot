using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMainBot.API
{
    internal static class Enums
    {
        public enum Roles : ulong
        {
            Verified,

            // Player
            Partner,
            Streamer,
            ServerBooster,

            //SubTeams
            Entwickler,
            Designer,
            Content,
            HeadContent,

            // LowTeam
            TestSupporter,
            JuniorSupporter,
            Supporter,
            Moderator,

            // HighTeam
            HeadModerator,
            Admin,
            Teamleitung,
            ProjectLeitung,
        }

        public static readonly Dictionary<ulong, Roles> RoleLookup = new()
        {
            { 1386702482868011180, Roles.Verified },

            // Player
            { 1360275258711609605, Roles.Partner },
            { 1360551949467910204, Roles.Streamer },
            { 1359833213178216559, Roles.ServerBooster },

            // SubTeams
            { 1329895338256896010, Roles.Entwickler },
            { 1330195846087573575, Roles.Designer },
            { 1329895298893479936, Roles.Content },
            { 1329895193167659098, Roles.HeadContent },

            // LowTeam
            { 1329906785754288138, Roles.TestSupporter },
            { 1332069937216225400, Roles.JuniorSupporter },
            { 1332069027899637792, Roles.Supporter },
            { 1329894665830400001, Roles.Moderator },

            // HighTeam
            { 1329897650954174474, Roles.HeadModerator },
            { 1329894427342143540, Roles.Admin },
            { 1329894247322882159, Roles.Teamleitung },
            { 1329869628406628482, Roles.ProjectLeitung },
        };


        public static string? ToRoleString(this Roles role)
        {
            switch (role)
            {
                default:
                    return null;
                case Roles.Verified:
                    return "Verifiziert";
                case Roles.Partner:
                    return "Server Partner";
                case Roles.Streamer:
                    return "Streamer";
                case Roles.ServerBooster:
                    return "Server Booster";
                case Roles.Entwickler:
                    return "Entwickler";
                case Roles.Designer:
                    return "Designer";
                case Roles.Content:
                    return "Content Team";
                case Roles.HeadContent:
                    return "Content Team Leitung";
                case Roles.TestSupporter:
                    return "Test Supporter";
                case Roles.JuniorSupporter:
                    return "Junior Supporter";
                case Roles.Supporter:
                    return "Supporter";
                case Roles.Moderator:
                    return "Moderator";
                case Roles.HeadModerator:
                    return "Head Moderator";
                case Roles.Admin:
                    return "Admin";
                case Roles.Teamleitung:
                    return "Teamleitung";
                case Roles.ProjectLeitung:
                    return "Projektleitung";
            }
        }
    }
}
