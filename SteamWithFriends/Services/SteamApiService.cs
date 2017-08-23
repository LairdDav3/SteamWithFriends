using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using SteamWithFriends.Models;
using System.Text.RegularExpressions;
using System.Net.Http;
using SteamWithFriends.Extensions;

namespace SteamWithFriends.Services
{
    public interface ISteamApiService
    {
        IEnumerable<SteamPlayer> GetPlayers(IEnumerable<string> userIds);
    }

    public class SteamApiService : ISteamApiService
    {
        private const string SteamApiUrl = "http://api.steampowered.com/";
        private const string SteamCommunityUrl = "http://steamcommunity.com/id/{0}/?xml=1";

        private static readonly HttpClient client = new HttpClient();

        public IEnumerable<SteamPlayer> GetPlayers(IEnumerable<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return null;


            var players = new HashSet<SteamPlayer>();
            var communityIds = new HashSet<string>();
            foreach(var id in userIds)
            {
                if(IsSteam64BitID(id))
                {
                    players.Add(new SteamPlayer { _64BitId = new SteamId(SteamIdType._64Bit, id) });
                    continue;
                }
                if(IsSteam32BitID(id))
                {
                    players.Add(new SteamPlayer
                    {
                        _32BitId = new SteamId(SteamIdType._32Bit, id),
                        _64BitId = new SteamId(SteamIdType._64Bit, this.Convert32BitIdTo64Bit(id))
                    });
                    continue;
                }
                communityIds.Add(id);
            }

            if(communityIds.Any())
            {
                var communityProfiles = Task.Run(() => GetCommunityProfiles(communityIds)).Result;
                foreach(var profileKvp in communityProfiles)
                {
                    if (profileKvp.Value == null)
                        players.Add(new SteamPlayer
                        {
                            _CommunityId = new SteamId(SteamIdType.Community, profileKvp.Key)
                        });
                    else
                        players.Add(new SteamPlayer
                        {
                            _32BitId = new SteamId(SteamIdType._32Bit, profileKvp.Value.steamID),
                            _64BitId = new SteamId(SteamIdType._64Bit, profileKvp.Value.steamID64),
                            _CommunityId = new SteamId(SteamIdType.Community, profileKvp.Key)
                        });
                }
            }

            return players;
        }

        private async Task<Dictionary<string, profile>> GetCommunityProfiles(IEnumerable<string> communityIds)
        {
            var communityProfiles = new Dictionary<string, profile>();
            var tasks = new Dictionary<string, Task<string>>();
            foreach (var commId in communityIds.Distinct())
            {
                var task = client.GetStringAsync(string.Format(SteamCommunityUrl, commId));
                task.Start();
                tasks.Add(commId, task);
            }

            await Task.WhenAll(tasks.Select(kvp => kvp.Value).ToArray());
            foreach(var taskKvp in tasks)
            {
                if (!taskKvp.Value.IsCompletedSuccessfully)
                    continue;
                var communityProfile = taskKvp.Value.Result.ToDeserialisedXml<profile>();
                communityProfiles.Add(taskKvp.Key, communityProfile);
            }

            return communityProfiles;
        }

        private bool IsSteam32BitID(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            return id.StartsWith("STEAM_");
            // Example 32bit profile: STEAM_1:0:12345678
        }

        private bool IsSteam64BitID(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            return id.Length == 16 && Regex.IsMatch(id, "^[0-9]");
        }

        private string Convert32BitIdTo64Bit(string _32bitId)
        {
            if (string.IsNullOrEmpty(_32bitId))
                return null;

            return Regex.Replace(_32bitId, "STEAM_[0-1]:[0-1]", "");
        }
    }
}
