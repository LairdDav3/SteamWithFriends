using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using SteamWithFriends.Models;

namespace SteamWithFriends.Services
{
    public interface ISteamApiService
    {
        IEnumerable<SteamPlayer> GetPlayers(IEnumerable<string> userIds);
    }

    public class SteamApiService : ISteamApiService
    {
        private const string SteamApiUrl = "http://api.steampowered.com/";

        public IEnumerable<SteamPlayer> GetPlayers(IEnumerable<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return null;



            throw new NotImplementedException();
        }
    }
}
