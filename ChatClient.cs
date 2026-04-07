using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkProgramming.Client
{
    public class ChatClient
    {
        private TcpClient client;
        private NetworkStream stream;

        public Action<string> OnMessageReceived;

        public void ConnectToServer(string ip, int port)
        {
            client = new TcpClient();
            client.Connect(ip, port);
            stream = client.GetStream();

            Thread thread = new Thread(ReceiveMessage);
            thread.IsBackground = true;
            thread.Start();
        }

        public void SendMessage(string message)
        {
            if (stream == null) return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void ReceiveMessage()
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        OnMessageReceived?.Invoke(msg);
                    }
                }
                catch
                {
                    Console.WriteLine("Disconnected");
                    break;
                }
            }
        }
    }
}