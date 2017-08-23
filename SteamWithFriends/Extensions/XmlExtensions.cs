using System.Xml.Serialization;

namespace SteamWithFriends.Extensions
{
    public static class XmlExtensions
    {
        public static T ToDeserialisedXml<T>(this string xmlString) where T: class
        {
            if (string.IsNullOrEmpty(xmlString))
                return null;

            var xmlSerialiser = new XmlSerializer(typeof(T));
            using (var reader = new System.IO.StringReader(xmlString))
            {
                return (T)xmlSerialiser.Deserialize(reader);
            }
        }
    }
}
