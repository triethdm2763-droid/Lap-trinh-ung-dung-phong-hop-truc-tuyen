using System;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // ⭐ Fix lỗi tiếng Việt + emoji
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Hello, World!");
        Console.WriteLine("Xin chào 😄🔥❤️");

        Console.ReadLine(); // giữ màn hình
    }
}
