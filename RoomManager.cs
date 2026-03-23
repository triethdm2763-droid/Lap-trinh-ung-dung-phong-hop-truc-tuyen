using System;
using System.Collections.Generic;
using NetworkingProgramming.Models;

namespace NetworkingProgramming.Managers
{
    public class RoomManager
    {
        private Dictionary<string, List<User>> rooms = new Dictionary<string, List<User>>();

        public void JoinRoom(string roomID, User user)
        {
            if (!rooms.ContainsKey(roomID))
            {
                rooms[roomID] = new List<User>();
            }

            rooms[roomID].Add(user);

            Broadcast(roomID, new Message("System", user.Username + " joined the room", MessageType.SYSTEM));
        }

        public void LeaveRoom(string roomID, User user)
        {
            if (rooms.ContainsKey(roomID))
            {
                rooms[roomID].Remove(user);

                Broadcast(roomID, new Message("System", user.Username + " left the room", MessageType.SYSTEM));
            }
        }

        public void Broadcast(string roomID, Message message)
        {
            Console.WriteLine(message.ToString());
        }

        public void ShowUsers(string roomID)
        {
            if (!rooms.ContainsKey(roomID)) return;

            Console.WriteLine("\nUsers in room " + roomID + ":");
            foreach (var user in rooms[roomID])
            {
                Console.WriteLine("- " + user.Username + (user.IsHost ? " (Host)" : ""));
            }
            Console.WriteLine();
        }
    }
}