using System;

namespace Network_Programming.Models
{
    public enum MessageType
    {
        JOIN,
        CHAT,
        LEAVE,
        SYSTEM,
        PRIVATE
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
            return Type switch
            {
                MessageType.JOIN   => $"[{Time:HH:mm:ss}] + {Sender} joined",
                MessageType.LEAVE  => $"[{Time:HH:mm:ss}] - {Sender} left",
                MessageType.SYSTEM => $"[{Time:HH:mm:ss}] (SYSTEM) {Content}",
                MessageType.PRIVATE => $"[{Time:HH:mm:ss}] (PRIVATE) {Sender}: {Content}",
                _ => $"[{Time:HH:mm:ss}] {Sender}: {Content}"
            };
        }
    }
}