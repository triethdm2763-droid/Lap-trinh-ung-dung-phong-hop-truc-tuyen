using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NetworkProgramming.Network
{
    public class SocketManager
    {
        private TcpClient client;
        private NetworkStream stream;

        private readonly ConcurrentQueue<string> sendQueue = new();
        private readonly object sendLock = new();

        private CancellationTokenSource cts;

        private string _ip;
        private int _port;

        public bool IsConnected => client?.Connected ?? false;

        public event Action<string> OnMessageReceived;
        public event Action OnDisconnected;

        // CONNECT
        public async Task Connect(string ip, int port)
        {
            _ip = ip;
            _port = port;

            client = new TcpClient();
            await client.ConnectAsync(ip, port);
            stream = client.GetStream();

            cts = new CancellationTokenSource();

            _ = ReceiveLoop(cts.Token);
            _ = SendLoop(cts.Token);
            _ = HeartbeatLoop(cts.Token);

            Console.WriteLine("[Socket] Connected");
        }

        // SEND
        public void Send(string msg)
        {
            if (!IsConnected) return;

            sendQueue.Enqueue(msg + "\n");
        }

        // SEND LOOP
        private async Task SendLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (sendQueue.TryDequeue(out var msg))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(msg);

                        lock (sendLock)
                        {
                            stream.Write(data, 0, data.Length);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("[Socket] Send error");
                }

                await Task.Delay(10);
            }
        }

        // RECEIVE LOOP
        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[4096];
            StringBuilder sb = new StringBuilder();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    int bytes = await stream.ReadAsync(buffer);
                    if (bytes == 0) break;

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes));

                    while (sb.ToString().Contains("\n"))
                    {
                        var data = sb.ToString();
                        int index = data.IndexOf("\n");

                        string completeMsg = data.Substring(0, index);
                        sb.Remove(0, index + 1);

                        OnMessageReceived?.Invoke(completeMsg);
                    }
                }
            }
            catch
            {
                Console.WriteLine("[Socket] Receive error");
            }

            HandleDisconnect();
        }

        // HEARTBEAT
        private async Task HeartbeatLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (IsConnected)
                    {
                        Send("{\"Type\":\"PING\"}");
                    }
                }
                catch { }

                await Task.Delay(5000);
            }
        }

        // DISCONNECT HANDLER
        private async void HandleDisconnect()
        {
            Console.WriteLine("[Socket] Disconnected");

            OnDisconnected?.Invoke();

            await Reconnect();
        }

        // AUTO RECONNECT
        private async Task Reconnect()
        {
            int retry = 0;

            while (retry < 5)
            {
                try
                {
                    Console.WriteLine($"[Reconnect] Attempt {retry + 1}");

                    await Connect(_ip, _port);

                    Console.WriteLine("[Reconnect] Success");
                    return;
                }
                catch
                {
                    retry++;
                    await Task.Delay(2000);
                }
            }

            Console.WriteLine("[Reconnect] Failed");
        }

        // MANUAL DISCONNECT
        public void Disconnect()
        {
            try
            {
                cts?.Cancel();
                stream?.Close();
                client?.Close();
            }
            catch { }
        }
    }
}
