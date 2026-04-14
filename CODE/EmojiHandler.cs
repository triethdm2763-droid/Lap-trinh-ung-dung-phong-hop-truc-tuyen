namespace Network_Programming.Features
{
    public static class EmojiHandler
    {
        public static string Parse(string msg)
        {
            return msg
                .Replace(":)", "😊")
                .Replace(":>", "😂")
                .Replace(":(", "😢")
                .Replace("<3", "❤️")
                .Replace(":D", "😄")
                .Replace(":fire", "🔥")
                .Replace(":thumbsup", "👍")
                .Replace(":cool", "😎")
                ;
            
        }
    }
}