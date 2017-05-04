using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Azalea.Networking
{
    public class NetHost
    {
        private const int MaxBuffer = 51200;

        private ServerDetail Detail;
        private TcpListener Listener;
        private Task AcceptClientTask;
        private bool IsRunning;

        public class Client {
            private string name;
            private string identifier;
            private IPAddress remoteIP;
            public IPAddress RemoteIP => remoteIP;
            private TcpClient connection;
            public TcpClient Connection => connection;
        }

        public NetHost(ServerDetail detail)
        {
            Detail = detail;

            Listener = new TcpListener(Detail.EndPoint);
            Listener.Start();

            IsRunning = true;
            AcceptClientTask = Task.Run(AcceptClient);
        }

        public async Task AcceptClient() 
        {
            while (IsRunning)
            {
                var client = await Listener.AcceptTcpClientAsync();
                var stream = client.GetStream();
                var serve = ServeClient(stream);
            }
        }

        public async Task ServeClient(NetworkStream stream)
        {
            var ReadBuffer = new Byte[MaxBuffer];
            await stream.ReadAsync(ReadBuffer, 0, MaxBuffer);
            // Process name, etc
            while(IsRunning) 
            {
                await stream.ReadAsync(ReadBuffer, 0, MaxBuffer);
            }
        }
    }
}
