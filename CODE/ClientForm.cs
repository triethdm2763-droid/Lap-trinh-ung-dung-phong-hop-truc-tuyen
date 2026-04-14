using System;

namespace NetworkProgramming.Client
{
    public class ClientForm
    {
        private ChatClient client = new ChatClient();

        public void JoinRoom()
        {
            Console.Write("Enter Server IP: ");
            string ip = Console.ReadLine();

            Console.Write("Enter Port: ");
            int port = int.Parse(Console.ReadLine());

            client.ConnectToServer(ip, port);
        }

        public void SendMessage()
        {
            while (true)
            {
                Console.Write(">> ");
                string msg = Console.ReadLine();

                if (msg.ToLower() == "exit")
                    break;

                client.SendMessage(msg);
            }
        }

        public void DisplayMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        public void UpdateUserList()
        {
            
        }

        public void Run()
        {
            JoinRoom();
            SendMessage();
        }
    }
}
