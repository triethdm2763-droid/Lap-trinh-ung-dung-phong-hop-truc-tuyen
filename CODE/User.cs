using System.Net.Sockets;

namespace Network_Programming.Models
{
    public class User
    {
        public string Username { get; set; }
        public string? RoomID { get; set; }
        public bool IsHost { get; set; }
        public TcpClient? Client { get; set; }

        // Constructor for server-side user (with client connection)
        public User(string username, TcpClient client)
        {
            Username = username;
            Client = client;
            IsHost = false;
        }

        // Constructor for UI display (username only)
        public User(string username)
        {
            Username = username;
            Client = null;
            IsHost = false;
        }
    }
}