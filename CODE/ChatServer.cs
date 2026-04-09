using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class ChatServer
    {
        private TcpListener server;
        private bool isRunning = false;

        public void Start(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;

            Console.WriteLine("Server started on port " + port);

            AcceptClient();
        }

        public void AcceptClient()
        {
            while (isRunning)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected!");

                ClientHandler handler = new ClientHandler(client);

                Thread t = new Thread(handler.HandleClient);
                t.Start();
            }
        }

        public void Stop()
        {
            isRunning = false;
            server.Stop();
            Console.WriteLine("Server stopped!");
        }
    }
}