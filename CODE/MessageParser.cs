namespace Network_Programming.Network
{
    public static class MessageParser
    {
        public static string Serialize(string type, string content)
        {
            return $"{type}|{content}";
        }

        public static (string type, string content) Deserialize(string msg)
        {
            var parts = msg.Split('|');
            return (parts[0], parts.Length > 1 ? parts[1] : "");
        }
    }
}