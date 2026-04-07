public static class FileTransfer
{
    public static bool IsFile(string msg)
    {
        return msg.StartsWith("/file");
    }

    public static string GetFileName(string msg)
    {
        return msg.Replace("/file ", "");
    }
}