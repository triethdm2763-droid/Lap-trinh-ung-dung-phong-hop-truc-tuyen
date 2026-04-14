using System;
using System.Collections.Generic;

namespace Network_Programming.Client
{
    public class ClientUIConsole
    {
        public string roomId = "";
        private string username = "";
        private List<string> users = new();
        private List<string> messages = new();
        private bool needsRerender = false;
        private bool isRendering = false;
        private object messageLock = new object();

        public void SetUsername(string name)
        {
            username = name;
        }

        public void UpdateRoom(string room, List<string> userList)
        {
            roomId = room;
            users = userList;
            needsRerender = true;
        }

        public void UpdateMessage(string msg)
        {
            lock (messageLock)
            {
                messages.Add(msg);

                if (messages.Count > 20)
                    messages.RemoveAt(0);
                
                needsRerender = true;
            }
        }

        public bool NeedsRerender()
        {
            return needsRerender;
        }

        public void ResetRerender()
        {
            needsRerender = false;
        }

        public bool IsRendering()
        {
            return isRendering;
        }

        public void SetRendering(bool value)
        {
            isRendering = value;
        }

        public void Render()
        {
            
            Console.WriteLine("+--------------------------------------------------------------+");
            Console.WriteLine("|                    ONLINE MEETING ROOM                       |");
            Console.WriteLine("+--------------------------------------------------------------+");
            Console.WriteLine($"| Room: {roomId,-17} Users: {users.Count,-9} You: {username,-15}|");
            Console.WriteLine("+--------------------------------------------------------------+");

            // CHAT BOX
            Console.WriteLine("| Chat                                                         |");
            Console.WriteLine("+--------------------------------------------------------------+");

            lock (messageLock)
            {
                int maxDisplay = 15;

                var displayMessages = messages.Count > maxDisplay
                    ? messages.Skip(messages.Count - maxDisplay)
                    : messages;

                foreach (var msg in displayMessages)
                {
                    string safeMsg = msg.Length > 60 ? msg.Substring(0, 57) + "..." : msg;
                    Console.WriteLine($"| {safeMsg,-60}|");
                }
            }

            Console.WriteLine("+--------------------------------------------------------------+");

            // USERS (ngắn gọn 1 dòng)
            string userLine = string.Join(", ", users);
            if (userLine.Length > 60)
                userLine = userLine.Substring(0, 57) + "...";

            Console.WriteLine($"| Users: {userLine,-54}|");
            Console.WriteLine("+--------------------------------------------------------------+");

            // INPUT
            Console.WriteLine("| exit | file <path> | private <user> <msg>                    |");
            Console.WriteLine("+--------------------------------------------------------------+");
            Console.Write(">> ");
        }
    }
}