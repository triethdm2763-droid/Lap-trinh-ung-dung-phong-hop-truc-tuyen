using System;

namespace NetworkingProgramming.Models
{
    public class User
    {
        public string Username { get; set; }
        public string RoomID { get; set; }
        public bool IsHost { get; set; }

        public User(string username, string roomID, bool isHost)
        {
            Username = username;
            RoomID = roomID;
            IsHost = isHost;
        }
    }
}
