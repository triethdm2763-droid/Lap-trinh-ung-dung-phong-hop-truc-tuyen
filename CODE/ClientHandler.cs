using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Network_Programming.Models;
using Network_Programming.Features;

namespace Network_Programming.Server
{
    public class ClientHandler
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private RoomManager roomManager;
        private User user;

        public ClientHandler(TcpClient client, RoomManager roomManager)
        {
            this.client = client;
            this.roomManager = roomManager;

            var stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };

            user = new User("Unknown", client);
        }

        public void HandleClient()
        {
            try
            {
                user.Username = reader.ReadLine() ?? "Unknown";

                string? msg;

                while ((msg = reader.ReadLine()) != null)
                {
                    Console.WriteLine("RECEIVED: " + msg);

                    if (msg == "CREATE")
                    {
                        string roomId = roomManager.CreateRoom();
                        user.IsHost = true;
                        user.RoomID = roomId;
                        roomManager.GetRoomUsers(roomId).Add(user);
                        Send($"ROOM_ID|{roomId}");
                        Console.WriteLine("SENT ROOM_ID: " + roomId);
                        continue;
                    }

                    if (msg.StartsWith("JOIN|"))
                    {
                        string roomId = msg.Split('|')[1].Trim();

                        bool ok = roomManager.JoinRoom(roomId, user);

                        if (!ok)
                        {
                            Send("ERROR|Room not found");
                        }
                        else
                        {
                            user.RoomID = roomId;
                            BroadcastSystem(roomId, $"{user.Username} joined");
                            BroadcastRoomInfo(roomId);
                            Send($"ROOM_INFO|{roomId}|0|{string.Join(",", roomManager.GetRoomUsers(roomId).Select(u => u.Username))}");
                        }

                        continue;
                    }

                    if (msg.StartsWith("KICK|"))
                    {
                        string target = msg.Split('|')[1];
                        if (user.IsHost && user.RoomID != null)
                        {
                            roomManager.KickUser(user.RoomID, target);
                        }
                        continue;
                    }

                    if (msg.StartsWith("CLOSE"))
                    {
                        if (user.IsHost && user.RoomID != null)
                        {
                            roomManager.CloseRoom(user.RoomID);
                        }
                        break;
                    }

                    if (msg.StartsWith("PRIVATE|"))
                    {
                        string[] parts = msg.Split('|');
                        if (parts.Length >= 3)
                        {
                            string targetUser = parts[1];
                            string privateMsg = string.Join("|", parts.Skip(2));

                            if (user.RoomID == null) continue;

                            var roomUsers = roomManager.GetRoomUsers(user.RoomID);
                            PrivateChat.SendPrivate(user, targetUser, privateMsg, roomUsers);
                        }
                        continue;
                    }

                    if (msg.StartsWith("FILE|"))
                    {
                        string[] parts = msg.Split('|');
                        if (parts.Length >= 4)
                        {
                            string fileName = parts[1];
                            string base64Data = parts[2];
                            string sender = parts[3];

                            // Nhận file và lưu vào thư mục Downloads
                            string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);
                            FileTransfer.ReceiveFile(client, fileName, base64Data, savePath);

                            // Broadcast thông báo file
                            if (user.RoomID != null)
                            {
                                roomManager.Broadcast(user.RoomID, $"[SYSTEM] {sender} sent file: {fileName}");
                            }
                        }
                        continue;
                    }

                    if (msg.StartsWith("FILE_PRIVATE|"))
                    {
                        string[] parts = msg.Split('|');
                        if (parts.Length >= 5)
                        {
                            string targetUser = parts[1];
                            string fileName = parts[2];
                            string base64Data = parts[3];
                            string sender = parts[4];

                            // Chỉ nhận nếu là target
                            if (user.Username == targetUser)
                            {
                                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);
                                FileTransfer.ReceiveFile(client, fileName, base64Data, savePath);

                                // Gửi thông báo private
                                Send($"[PRIVATE] {sender} sent file: {fileName}");
                            }
                        }
                        continue;
                    }

                    // Chat message
                    if (user.RoomID != null)
                    {
                        roomManager.Broadcast(user.RoomID, $"{user.Username}: {msg}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SERVER ERROR: " + ex.Message);
            }
            finally
            {
                roomManager.LeaveRoom(user);
                client.Close();
            }
        }

        private void Send(string msg)
        {
            writer.WriteLine(msg);
        }

        private void BroadcastSystem(string roomId, string msg)
        {
            var users = roomManager.GetRoomUsers(roomId);
            foreach (var u in users)
            {
                try
                {
                    if (u.Client == null) continue;

                    var streamWriter = new StreamWriter(u.Client.GetStream())
                    {
                        AutoFlush = true
                    };

                    streamWriter.WriteLine("[SYSTEM] " + msg);
                }
                catch { }
            }
        }

        private void BroadcastRoomInfo(string roomId)
        {
            var users = roomManager.GetRoomUsers(roomId).Select(u => u.Username).ToList();
            string userList = string.Join(",", users);

            var roomUsers = roomManager.GetRoomUsers(roomId);
            foreach (var u in roomUsers)
            {
                try
                {
                    if (u.Client == null) continue;

                    var streamWriter = new StreamWriter(u.Client.GetStream())
                    {
                        AutoFlush = true
                    };

                    streamWriter.WriteLine($"ROOM_INFO|{roomId}|{users.Count}|{userList}");
                }
                catch { }
            }
        }
    }
}