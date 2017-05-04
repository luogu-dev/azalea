﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Azalea.Networking
{
    public class Broadcast
    {
        private const int DefaultListenPort = 6391;
        private const int BindPort = 6392;
        private const int BroadcastDelay = 200;
        private const int SeekTimeout = 3000;

        private IPEndPoint SendEndpoint;
        private IPEndPoint ListenEndpoint;

        private static Broadcast instance = new Broadcast();
        public static Broadcast Instance => instance;

        public Broadcast()
        {
            SendEndpoint = new IPEndPoint(IPAddress.Broadcast, BindPort);
            ListenEndpoint = new IPEndPoint(IPAddress.Any, BindPort);
        }

        private async Task Send(ServerDetail data)
        {
            var client = new UdpClient();
            var dataString = data.Serialize();
            var bytes = Encoding.ASCII.GetBytes(dataString);
            await client.SendAsync(bytes, bytes.Length, SendEndpoint);
        }

        private async Task<ServerDetail> Receive()
        {
            var client = new UdpClient(ListenEndpoint);
            var asyncData = await client.ReceiveAsync();
            var data = Encoding.ASCII.GetString(asyncData.Buffer);
            try
            {
                return ServerDetail.Unserialize(data);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public async Task<ServerDetail> GetServer() 
        {
            var task = Receive();
            if (await Task.WhenAny(task, Task.Delay(SeekTimeout)) == task) 
            {
                return task.Result;
            } 
            else 
            {
                return null;
            }
        }

        private void SendTimer(Object data) 
        {
            var ServerDetail = (ServerDetail)data;
            var task = Send(ServerDetail);
        }

        public Timer StartBroadcast(ServerDetail detail) 
        {
            var timer = new Timer(this.SendTimer, detail, 0, BroadcastDelay);
            return timer;
        }
    }
}
