using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Server
{
    class ClientHandler
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;

        private string username;
        private string currentRoom = null;

        // ===== DATA =====
        private static Dictionary<string, string> roomPasswords = new Dictionary<string, string>();
        private static Dictionary<string, List<ClientHandler>> rooms = new Dictionary<string, List<ClientHandler>>();

        public ClientHandler(TcpClient client)
        {
            this.client = client;

            NetworkStream stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.AutoFlush = true;
        }

        // ===== HANDLE CLIENT =====
        public void HandleClient()
        {
            try
            {
                username = ReceiveMessage();
                Console.WriteLine(username + " connected");

                while (true)
                {
                    string msg = ReceiveMessage();
                    if (msg == null) break;

                    // ===== CREATE ROOM =====
                    if (msg.StartsWith("CREATE_ROOM"))
                    {
                        string[] p = msg.Split('|');
                        string room = p[1];
                        string pass = p[2];

                        if (!rooms.ContainsKey(room))
                        {
                            rooms[room] = new List<ClientHandler>();
                            roomPasswords[room] = pass;

                            SendMessage("CREATE_OK");
                            Console.WriteLine(username + " created room " + room);
                        }
                        else
                        {
                            SendMessage("ROOM_EXISTS");
                        }
                    }

                    // ===== JOIN ROOM =====
                    else if (msg.StartsWith("JOIN_ROOM"))
                    {
                        string[] p = msg.Split('|');
                        string room = p[1];
                        string pass = p[2];

                        if (rooms.ContainsKey(room))
                        {
                            if (roomPasswords[room] == pass)
                            {
                                currentRoom = room;
                                rooms[room].Add(this);

                                SendMessage("JOIN_OK");
                                Console.WriteLine(username + " joined room " + room);

                                Broadcast(room, "[SERVER] " + username + " joined room");
                            }
                            else
                            {
                                SendMessage("WRONG_PASS");
                            }
                        }
                        else
                        {
                            SendMessage("ROOM_NOT_FOUND");
                        }
                    }

                    // ===== SEND MESSAGE =====
                    else if (msg.StartsWith("SEND_MESSAGE"))
                    {
                        string content = msg.Split('|')[1];

                        if (currentRoom != null)
                        {
                            string full = username + ": " + content;
                            Broadcast(currentRoom, full);
                        }
                    }
                }
            }
            catch { }

            Console.WriteLine(username + " disconnected");
            client.Close();
        }

        // ===== RECEIVE =====
        public string ReceiveMessage()
        {
            return reader.ReadLine();
        }

        // ===== SEND =====
        public void SendMessage(string msg)
        {
            writer.WriteLine(msg);
        }

        // ===== BROADCAST =====
        private void Broadcast(string room, string msg)
        {
            foreach (var c in rooms[room])
            {
                c.SendMessage(msg);
            }
        }
    }
}
