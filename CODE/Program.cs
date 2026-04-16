using Network_Programming.Server;
using Network_Programming.Client;
using Network_Programming.Models;
using System.Net.Sockets;
using Network_Programming.Host;
using Network_Programming.Features;


class Program
{
    static void Main()
    {
        Console.WriteLine("===== ONLINE MEETING SYSTEM =====");
        Console.WriteLine("1. Server");
        Console.WriteLine("2. Host");
        Console.WriteLine("3. Client");
        Console.Write("Choose: ");
       
        var choice = Console.ReadLine();

        // =========================
        // 1. SERVER
        // =========================
        if (choice == "1")
        {
            var server = new ChatServer(8080);
            server.Start();

            Console.WriteLine("Server running on port 8080...");
            Console.ReadLine();
        }
        // =========================
        // 2. HOST
        // =========================
        else if (choice == "2")
        {
            Console.Write("Username: ");
            string username = Console.ReadLine() ?? "Host";
            Console.Clear();
            try
            {
                string ip = "127.0.0.1";
                int port = 8080;

                TcpClient client = new TcpClient(ip, port);
                //Console.WriteLine("Connected to server!");

                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream)
                {
                    AutoFlush = true
                };

                writer.WriteLine(username);
                writer.WriteLine("CREATE"); 

                string? roomIdResponse = reader.ReadLine();
                if (roomIdResponse != null && roomIdResponse.StartsWith("ROOM_ID|"))
                {
                    string roomId = roomIdResponse.Split('|')[1];

                    HostConsoleUI hostUI = new HostConsoleUI();
                    hostUI.SetRoom(roomId);
                    hostUI.SetUsername(username);

                    // Thread nhận message
                    new Thread(() =>
                    {
                        while (true)
                        {
                            string? msg = reader.ReadLine();
                            if (msg != null)
                            {
                                if (msg.StartsWith("ROOM_INFO|"))
                                {
                                    var parts = msg.Split('|');
                                    if (parts.Length >= 4)
                                    {
                                        var userList = parts[3].Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList();
                                        hostUI.UpdateUsers(userList.Select(u => new User(u)).ToList());
                                        hostUI.UpdateMessage($"[SYSTEM] Room info updated: {userList.Count} users");
                                    }
                                }
                                else
                                {
                                    hostUI.UpdateMessage(msg);
                                }
                            }
                        }
                    })
                    { IsBackground = true }.Start();

                    // Menu loop
                    while (true)
                    {
                        if (hostUI.needsChatRerender)
                        {
                            hostUI.Render();
                            hostUI.needsChatRerender = false;
                        }
                        else
                        {
                            hostUI.Render();
                        }

                        string input = Console.ReadLine() ?? "";

                        if (input == "1")
                        {
                            string kickUser = hostUI.AskKickUser();
                            if (!string.IsNullOrEmpty(kickUser))
                            {
                                writer.WriteLine($"KICK|{kickUser}");
                            }
                        }
                        else if (input == "2")
                        {
                            // Refresh
                        }
                        else if (input == "3")
                        {
                            writer.WriteLine("CLOSE");
                            break;
                        }
                        else if (input == "4")
                        {
                            // Enter Chat Mode
                            bool inChatMode = true;
                            hostUI.needsChatRerender = true;

                            // Thread render UI real-time
                            new Thread(() =>
                            {
                                while (inChatMode)
                                {
                                    if (hostUI.needsChatRerender)
                                    {
                                        hostUI.RenderChat();
                                        hostUI.needsChatRerender = false;
                                        Thread.Sleep(100); // Tránh nhấp nháy quá nhanh
                                    }
                                    Thread.Sleep(50); // Check mỗi 50ms
                                }
                            })
                            { IsBackground = true }.Start();

                            while (inChatMode)
                            {
                                string chatInput = Console.ReadLine() ?? "";

                                if (chatInput.ToLower() == "exit")
                                {
                                    inChatMode = false;
                                }
                                else if (chatInput.StartsWith("file "))
                                {
                                    string filePath = chatInput.Substring(5).Trim();
                                    if (File.Exists(filePath))
                                    {
                                        FileTransfer.SendFile(client, filePath, null, "Host");
                                    }
                                    else
                                    {
                                        Console.WriteLine("File không tồn tại!");
                                    }
                                }
                                else if (chatInput.StartsWith("private "))
                                {
                                    var parts = chatInput.Split(' ', 3);
                                    if (parts.Length >= 3)
                                    {
                                        string target = parts[1];
                                        string message = parts[2];
                                        writer.WriteLine($"PRIVATE|{target}|{message}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Cú pháp: private <user> <message>");
                                    }
                                }
                                else if (!string.IsNullOrEmpty(chatInput))
                                {
                                    writer.WriteLine(chatInput);
                                }
                            }
                        }
                    }

                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        // =========================
        // 3. CLIENT
        // =========================
        else if (choice == "3")
        {
            while (true) // Loop để nhập link lại nếu sai
            {
                Console.Write("Username: ");
                string username = Console.ReadLine() ?? "Unknown";

                Console.Write("Paste room link (format: 127.0.0.1:8080/roomId): ");
                string link = Console.ReadLine() ?? "";

                link = link.Replace("meet://", "").Trim();

                var parts = link.Split('/');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Link không hợp lệ!");
                    continue;
                }

                string addressPart = parts[0].Trim();
                string roomId = parts[1].Trim();

                var address = addressPart.Split(':');
                if (address.Length != 2)
                {
                    Console.WriteLine("Address không hợp lệ!");
                    continue;
                }

                string ip = address[0];
                int port = int.Parse(address[1]);

                try
                {
                    TcpClient client = new TcpClient(ip, port);
                    Console.WriteLine("Connected to server!");

                    NetworkStream stream = client.GetStream();
                    StreamReader reader = new StreamReader(stream);
                    StreamWriter writer = new StreamWriter(stream)
                    {
                        AutoFlush = true
                    };

                    writer.WriteLine(username);
                    writer.WriteLine($"JOIN|{roomId}");

                    // Check join response
                    string? joinResponse = reader.ReadLine();
                    if (joinResponse == "ERROR|Room not found")
                    {
                        Console.WriteLine("Room not found!");
                        client.Close();
                        continue;
                    }

                    ClientUIConsole ui = new ClientUIConsole();
                    ui.SetUsername(username);

                    // Thread nhận message
                    new Thread(() =>
                    {
                        while (true)
                        {
                            string? msg = reader.ReadLine();
                            if (msg != null)
                            {
                                if (msg.StartsWith("ROOM_INFO|"))
                                {
                                    var msgParts = msg.Split('|');
                                    if (msgParts.Length >= 4)
                                    {
                                        string room = msgParts[1];
                                        var userList = msgParts[3].Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList();
                                        ui.UpdateRoom(room, userList);
                                    }
                                }
                                else if (msg.StartsWith("USERS|"))
                                {
                                    var userList = msg.Split('|')[1].Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList();
                                    ui.UpdateRoom(ui.roomId, userList);
                                }
                                else
                                {
                                    ui.UpdateMessage(msg);
                                }
                            }
                        }
                    })
                    { IsBackground = true }.Start();

                    // Thread render UI real-time
                    new Thread(() =>
                    {
                        while (true)
                        {
                            if (!ui.IsRendering() && ui.NeedsRerender())
                            {
                                ui.SetRendering(true);
                                ui.Render();
                                ui.ResetRerender();
                                ui.SetRendering(false);
                                Thread.Sleep(100); // Tránh nhấp nháy quá nhanh
                            }
                            Thread.Sleep(50); // Check mỗi 50ms
                        }
                    })
                    { IsBackground = true }.Start();

                    // Loop gửi chat
                    while (true)
                    {
                        string input = Console.ReadLine() ?? "";
                        
                        if (input.ToLower() == "exit")
                        {
                            break;
                        }
                        
                        if (input.StartsWith("file "))
                        {
                            string filePath = input.Substring(5).Trim();
                            if (File.Exists(filePath))
                            {
                                FileTransfer.SendFile(client, filePath, null, username);
                            }
                            else
                            {
                                Console.WriteLine("File không tồn tại!");
                            }
                        }
                        else if (input.StartsWith("private "))
                        {
                            var parts_input = input.Split(' ', 3);
                            if (parts_input.Length >= 3)
                            {
                                string target = parts_input[1];
                                string message = parts_input[2];
                                writer.WriteLine($"PRIVATE|{target}|{message}");
                            }
                            else
                            {
                                Console.WriteLine("Cú pháp: private <user> <message>");
                            }
                        }
                        else if (!string.IsNullOrEmpty(input))
                        {
                            writer.WriteLine(input);
                        }
                    }

                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi kết nối: " + ex.Message);
                }
            }
        }
    }
}
