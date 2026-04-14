using System;
using System.Collections.Generic;
using System.Linq;
using Network_Programming.Models;
using Network_Programming.Features;
using System.IO;

namespace Network_Programming.Server
{
    public class RoomManager
    {
        private Dictionary<string, List<User>> rooms = new();

        public List<User> GetRoomUsers(string roomId)
        {
            if (!rooms.ContainsKey(roomId))
                return new List<User>();

            return rooms[roomId];
        }

        public string CreateRoom()
        {
            string roomId = Guid.NewGuid().ToString().Substring(0, 6);
            rooms[roomId] = new List<User>();
            return roomId;
        }

        public bool JoinRoom(string roomId, User user)
        {
            if (!rooms.ContainsKey(roomId)) 
                return false;

            user.RoomID = roomId;
            rooms[roomId].Add(user);
            return true;
        }

        public void LeaveRoom(User user)
        {
            if (user.RoomID == null) return;

            if (rooms.ContainsKey(user.RoomID))
            {
                rooms[user.RoomID].Remove(user);
                if (rooms[user.RoomID].Count == 0)
                {
                    rooms.Remove(user.RoomID);
                }
                else
                {
                    BroadcastSystem(user.RoomID, $"{user.Username} left");
                    BroadcastRoomInfo(user.RoomID);
                }
            }
        }

        public void Broadcast(string roomId, string msg)
        {
            if (!rooms.ContainsKey(roomId)) return;

            msg = EmojiHandler.Parse(msg);

            // Thêm thời gian Việt Nam
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTime(DateTime.Now, vietnamTimeZone);
            string timeStr = vietnamTime.ToString("HH:mm:ss");
            msg = $"{timeStr} : {msg}";

            foreach (var u in rooms[roomId])
            {
                try
                {
                    if (u.Client == null)
                        continue;

                    var writer = new StreamWriter(u.Client.GetStream())
                    {
                        AutoFlush = true
                    };

                    writer.WriteLine(msg);
                }
                catch { }
            }
        }

        public void KickUser(string roomId, string username)
        {
            if (!rooms.ContainsKey(roomId)) return;

            var user = rooms[roomId].FirstOrDefault(x => x.Username == username);
            if (user == null) return;

            rooms[roomId].Remove(user);

            Send(user, "[SYSTEM] You were kicked");
            
            if (user.Client != null)
            {
                user.Client.Close();
            }

            BroadcastSystem(roomId, $"{username} was kicked");
            BroadcastRoomInfo(roomId);
        }

        public void CloseRoom(string roomId)
        {
            if (!rooms.ContainsKey(roomId)) return;

            foreach (var user in rooms[roomId])
            {
                Send(user, "[SYSTEM] Room has been closed");
                user.Client?.Close();
            }

            rooms.Remove(roomId);
        }

        private void BroadcastSystem(string roomId, string msg)
        {
            if (!rooms.ContainsKey(roomId)) return;
            
            foreach (var u in rooms[roomId])
                Send(u, "[SYSTEM] " + msg);
        }

        private void BroadcastRoomInfo(string roomId)
        {
            if (!rooms.ContainsKey(roomId)) return;
            
            var users = GetRoomUsers(roomId).Select(u => u.Username).ToList();
            string userList = string.Join(",", users);

            foreach (var u in rooms[roomId])
            {
                Send(u, $"ROOM_INFO|{roomId}|{users.Count}|{userList}");
            }
        }

        private void Send(User u, string msg)
        {
            try
            {   
                if (u.Client == null)
                    return;

                var writer = new StreamWriter(u.Client.GetStream())
                {
                    AutoFlush = true
                };

                writer.WriteLine(msg);
            }
            catch { }
        }

        public List<User> GetUsers(string roomId)
        {
            if (!rooms.ContainsKey(roomId)) return new();
            return rooms[roomId];
        }
    }
}