public static class EmojiHandler
{
    public static bool IsEmojiMessage(string msg)
    {
        return msg.StartsWith("/emoji");
    }

    public static string ParseEmoji(string msg)
    {
        return msg.Replace("/emoji ", "");
    }
}
