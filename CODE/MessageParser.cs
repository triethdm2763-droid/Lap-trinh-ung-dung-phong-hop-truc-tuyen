using System;
using System.Text.Json;
using NetworkingProgramming.Models;

namespace NetworkingProgramming.Utils
{
    public static class MessageParser
    {
        public static string Serialize(Message msg)
        {
            return JsonSerializer.Serialize(msg);
        }
        public static Message Deserialize(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Message>(json);
            }
            catch
            {
                return new Message("System", "Invalid message format", MessageType.SYSTEM);
            }
        }
        public static Message CreateChat(string sender, string content)
        {
            return new Message(sender, content, MessageType.CHAT);
        }

        public static Message CreateSystem(string content)
        {
            return new Message("System", content, MessageType.SYSTEM);
        }

        public static Message CreateJoin(string user)
        {
            return new Message("System", user + " joined", MessageType.JOIN);
        }

        public static Message CreateLeave(string user)
        {
            return new Message("System", user + " left", MessageType.LEAVE);
        }
    }
}
