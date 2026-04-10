using System;

namespace NetworkingProgramming.Models
{
    public enum MessageType
    {
        JOIN,
        CHAT,
        LEAVE,
        SYSTEM
    }

    public class Message
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public MessageType Type { get; set; }

        public Message(string sender, string content, MessageType type)
        {
            Sender = sender;
            Content = content;
            Type = type;
            Time = DateTime.Now;
        }

        public override string ToString()
        {
            if (Type == MessageType.SYSTEM)
                return "[" + Time.ToString("HH:mm:ss") + "] " + Content;

            return "[" + Time.ToString("HH:mm:ss") + "] "
                   + Sender + ": " + Content;
        }
    }
}
