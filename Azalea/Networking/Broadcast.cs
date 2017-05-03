using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Azalea.Networking
{
    public class Broadcast
    {
        private const int Port = 6392;
        private IPEndPoint SendEndpoint;
        private IPEndPoint ListenEndpoint;

        public struct Message
        {
            public string Name;
            public string Identifier;
            public IPEndPoint EndPoint;
            public string IPAddress 
            {
                get 
                {

                }
                set 
                {
                }
            }
        }

        public Broadcast()
        {
            SendEndpoint = new IPEndPoint(IPAddress.Broadcast, Port);
            ListenEndpoint = new IPEndPoint(IPAddress.Any, Port);
        }

        public void Send(Message data)
        {
        }

        public async Task<string> Receive()
        {
            var client = new UdpClient(ListenEndpoint);
            var asyncData = await client.ReceiveAsync();
            var data = System.Text.Encoding.UTF8.GetString(asyncData.Buffer);
            return data;
        }
    }
}
