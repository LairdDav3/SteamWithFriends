using System.Collections.Generic;

namespace SteamWithFriends.Models
{

    public class SteamProfileIndexViewModel
    {
        public ICollection<string> Profiles { get; set; }

        public IEnumerable<SteamPlayer> Results { get; set; }
    }
}
