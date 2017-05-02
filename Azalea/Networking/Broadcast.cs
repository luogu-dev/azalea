using System;
using System.Net;

namespace Azalea.Networking
{
    public class Broadcast
    {
        private const int Port = 6392;
        private IPEndPoint SendEndpoint;
        private IPEndPoint ListenEndpoint;

        public Broadcast()
        {
            SendEndpoint = new IPEndPoint(IPAddress.Broadcast, Port);
            ListenEndpoint = new IPEndPoint(IPAddress.Any, Port);
        }

        public void Send() {
            
        }

    }
}
