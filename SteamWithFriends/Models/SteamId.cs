namespace SteamWithFriends.Models
{
    public class SteamId
    {
        public string Id { get; set; }
        public SteamIdType Type { get; set; }

        public SteamId()
        {

        }
        public SteamId(SteamIdType type, string id)
        {
            Type = type;
            Id = id;
        }
    }

    public enum SteamIdType
    {
        _32Bit,
        _64Bit,
        Community,
    }
}
