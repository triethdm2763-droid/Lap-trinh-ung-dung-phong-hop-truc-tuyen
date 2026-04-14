using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Network_Programming.Server
{
    public class ChatServer
    {
        private TcpListener server;
        private bool running = false;
        private RoomManager roomManager = new();

        public RoomManager RoomManager => roomManager;

        public ChatServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            server.Start();
            running = true;

            Console.WriteLine("Server started...");

            new Thread(AcceptClient) { IsBackground = true }.Start();
        }

        private void AcceptClient()
        {
            while (running)
            {
                try
                {
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected");

                    var handler = new ClientHandler(client, roomManager);
                    new Thread(handler.HandleClient) { IsBackground = true }.Start();
                }
                catch
                {
                    break;
                }
            }
        }

        public void Stop()
        {
            running = false;
            server.Stop();
        }
    }
}