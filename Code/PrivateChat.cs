public static class PrivateChat
{
    public static bool IsPrivate(string msg)
    {
        return msg.StartsWith("/to");
    }

    public static (string to, string content) Parse(string msg)
    {
        var parts = msg.Split(' ', 3);
        return (parts[1], parts[2]);
    }
}
