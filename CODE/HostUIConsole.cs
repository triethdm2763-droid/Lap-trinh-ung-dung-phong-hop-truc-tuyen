using System;
using System.Collections.Generic;
using Network_Programming.Models;

namespace Network_Programming.Host
{
    public class HostConsoleUI
    {
        private string roomId = "";
        private string username = "";
        private List<User> users = new();
        private List<string> messages = new();
        public bool needsChatRerender = true;
        private object messageLock = new object();

        public void SetRoom(string id)
        {
            roomId = id;
        }

        public void SetUsername(string name)
        {
            username = name;
        }

        public void UpdateUsers(List<User> list)
        {
            users = list;
        }

        public void UpdateMessage(string msg)
        {
            lock (messageLock)
            {
                messages.Add(msg);

                if (messages.Count > 20)
                    messages.RemoveAt(0);
                
                needsChatRerender = true;
            }
        }

        public void Render()
        {
        
            Console.WriteLine("+--------------------------------------------------------------+");
            Console.WriteLine("|                    HOST CONTROL PANEL                        |");
            Console.WriteLine("+--------------------------------------------------------------+");
            Console.WriteLine($"| Room ID: {roomId,-52}|");
            Console.WriteLine($"| Invite: 127.0.0.1:8080/{roomId,-38}|"); 
            Console.WriteLine("+--------------------------+-----------------------------------+");
            Console.WriteLine($"| Room: {roomId,-15} Users: {users.Count}/100       You: {username,-15}|");
            Console.WriteLine("+--------------------------+-----------------------------------+");

            Console.WriteLine("| Participants             | Chat                              |");
            Console.WriteLine("+--------------------------+-----------------------------------+");

            int maxRows = Math.Max(users.Count, messages.Count);

            for (int i = 0; i < maxRows; i++)
            {
                string userText = "";
                string chatText = "";

                if (i < users.Count)
                {
                    var u = users[i];
                    userText = u.Username + (u.IsHost ? " (Host)" : "");
                }

                lock (messageLock)
                {
                    if (i < messages.Count)
                    {
                        chatText = messages[i];
                    }
                }

                Console.WriteLine($"| {userText,-24} | {chatText,-33} |");
            }

            Console.WriteLine("+--------------------------+-----------------------------------+");
            Console.WriteLine("| [1] Kick  [2] Refresh  [3] Exit  [4] Chat                    |");
            Console.WriteLine("+--------------------------------------------------------------+");
            Console.Write("Choose: ");
        }

        public void RenderChat()
{
    Console.WriteLine("+--------------------------------------------------------------+");
    Console.WriteLine("|                        CHAT MODE                             |");
    Console.WriteLine("+--------------------------------------------------------------+");
    Console.WriteLine($"| Room ID: {roomId,-52}|");
    Console.WriteLine($"| Invite: 127.0.0.1:8080/{roomId,-38}|"); 
    Console.WriteLine("+--------------------------+-----------------------------------+");
    Console.WriteLine($"| Room: {roomId,-15} Users: {users.Count}/100       You: {username,-15}|");
    Console.WriteLine("+--------------------------------------------------------------+");

   
    Console.WriteLine("| Chat                                                        |");
    Console.WriteLine("+--------------------------------------------------------------+");

    lock (messageLock)
    {
        int maxDisplay = 15;

        var displayMessages = messages.Count > maxDisplay
            ? messages.Skip(messages.Count - maxDisplay)
            : messages;

        foreach (var msg in displayMessages)
        {
            // cắt nếu quá dài
            string safeMsg = msg.Length > 60 ? msg.Substring(0, 57) + "..." : msg;
            Console.WriteLine($"| {safeMsg,-60}|");
        }
    }

    Console.WriteLine("+--------------------------------------------------------------+");

    // HƯỚNG DẪN
    Console.WriteLine("| exit | file <path> | private <user> <msg>                    |");
    Console.WriteLine("+--------------------------------------------------------------+");

    // INPUT
    Console.Write(">> ");
}

        public string GetChatInput()
        {
            return Console.ReadLine() ?? "";
        }

        public string AskKickUser()
        {
            Console.Write("Enter username to kick: ");
            return Console.ReadLine() ?? "";
        }

        public string AskMessage()
        {
            Console.Write("Message: ");
            return Console.ReadLine() ?? "";
        }
    }
}
