using System.Net.Sockets;
using System.IO;

namespace Network_Programming.Network
{
    public class SocketManager
    {
        private TcpClient? client;
        private StreamReader? reader;
        private StreamWriter? writer;

        public Action<string>? OnMessageReceived;

        public void Connect(string ip, int port, string username)
        {
            client = new TcpClient(ip, port);

            writer = new StreamWriter(client.GetStream())
            {
                AutoFlush = true
            };

            reader = new StreamReader(client.GetStream());

            // gửi username ngay khi connect
            writer.WriteLine(username);
        }

        public void Send(string msg)
        {
            writer?.WriteLine(msg);
        }

        public string? Receive()
        {
            return reader?.ReadLine();
        }
    }
}