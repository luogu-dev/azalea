using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using Azalea.Exceptions;

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

        public class Message
        {
            private string name;
            public string Name 
            {
                get
                {
                    return name;
                }
                private set
                {
                    if (value.Length == 0)
                    {
                        throw new ArgumentNullException();
                    }

                    if (!Regex.IsMatch(value, @"^[a-zA-Z0-9\-_]{3,15}$"))
                    {
                        throw new InvalidArgumentException("Invalid Name");
                    }

                    name = value;
                }
            }

            private string identifier;
            public string Identifier 
            {
                get
                {
                    return identifier;
                }
                private set
                {
					if (!Regex.IsMatch(value, @"^[A-F0-9]{12}$"))
					{
						throw new InvalidArgumentException("Invalid Identifier");
					}

                    identifier = value;
                }
            }

            public IPEndPoint EndPoint;
            public string IPAddress 
            {
                get 
                {
                    return EndPoint.Address.ToString();
                }
                private set 
                {
                    EndPoint.Address = System.Net.IPAddress.Parse(value);
                }
            }
            public int Port
            {
                get
                {
                    return EndPoint.Port;
                }
                private set
                {
                    EndPoint.Port = value;
                }
            }

            public Message(string _name, string _identifier, IPEndPoint _endPoint) 
            {
				Name = _name;
                Identifier = _identifier;
				EndPoint = _endPoint;
            }

            public Message(string _name, string _identifier, string ipAddress, int port) 
            {
                var endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
				Name = _name;
				Identifier = _identifier;
				EndPoint = endPoint;
            }

            public string Serialize() 
            {
                var stringArray = new List<String> { Name, Identifier, IPAddress, Port.ToString() };
                return String.Join(";", stringArray);
            }

            public static Message Unserialize(string data) 
            {
                var stringArray = data.Split(';');
                if (stringArray.Length != 4) 
                {
                    throw new InvalidArgumentException("Invalid MessageData");
                }

                int port;
                if (!Int32.TryParse(stringArray[3], out port)) {
                    throw new InvalidArgumentException("Port in MessageData is not a valid Int");
                }

                return new Message(stringArray[0], stringArray[1], stringArray[2], port);
            }
        }

        public Broadcast()
        {
            SendEndpoint = new IPEndPoint(IPAddress.Broadcast, BindPort);
            ListenEndpoint = new IPEndPoint(IPAddress.Any, BindPort);
        }

        private async Task Send(Message data)
        {
            var client = new UdpClient();
            var dataString = data.Serialize();
            var bytes = Encoding.ASCII.GetBytes(dataString);
            await client.SendAsync(bytes, bytes.Length, SendEndpoint);
        }

        private async Task<Message> Receive()
        {
            var client = new UdpClient(ListenEndpoint);
            var asyncData = await client.ReceiveAsync();
            var data = Encoding.ASCII.GetString(asyncData.Buffer);
            try
            {
                return Message.Unserialize(data);
            }
            catch (InvalidArgumentException)
            {
                return null;
            }
        }

        public async Task<Message> GetServer() {
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

        private void SendTimer(Object data) {
            var message = (Message)data;
            var task = Send(message);
        }

        public Timer StartBroadcast() {
            var dummyMessage = new Message("Test", "AABBCCDDEEFF", "127.0.0.1", DefaultListenPort);
            var timer = new Timer(this.SendTimer, dummyMessage, 0, BroadcastDelay);
            return timer;
        }
    }
}
