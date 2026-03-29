using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ChatClient
{
    TcpClient client;
    NetworkStream stream;

    public Action<string> OnMessageReceived;

    public void ConnectToServer(string ip, int port)
    {
        client = new TcpClient(ip, port);
        stream = client.GetStream();

        Thread t = new Thread(ReceiveMessage);
        t.Start();
    }

    public void SendMessage(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        stream.Write(data, 0, data.Length);
    }

    void ReceiveMessage()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytes = stream.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

            OnMessageReceived?.Invoke(msg);
        }
    }
}