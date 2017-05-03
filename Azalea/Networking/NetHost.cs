using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Azalea.Networking
{
    public class NetHost
    {
        private ServerDetail Detail;
        private TcpListener Listener;
        private Task AcceptClientTask;
        private bool IsRunning;

        public NetHost(ServerDetail detail)
        {
            Detail = detail;

            Listener = new TcpListener(Detail.EndPoint);
            Listener.Start();

            IsRunning = true;
            AcceptClientTask = Task.Run(AcceptClient);
        }

        public async Task AcceptClient() {
            while (IsRunning)
            {
                var client = await Listener.AcceptTcpClientAsync();

            }
        }


    }
}
