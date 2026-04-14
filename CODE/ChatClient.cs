using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Network_Programming.Client
{
    public class ChatClient
    {
        private TcpClient? client;
        private StreamReader? reader;
        private StreamWriter? writer;

        public void Connect(string ip, int port, string username)
        {
            client = new TcpClient(ip, port);

            var stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };

            writer.WriteLine(username);

            new Thread(Receive).Start();
        }

        public void Send(string msg)
        {
            writer?.WriteLine(msg);
        }

        private void Receive()
        {
            while (true)
            {
                string? msg = reader?.ReadLine();
                if (msg != null)
                {
                    Console.WriteLine(msg);
                }
            }
        }
    }
}