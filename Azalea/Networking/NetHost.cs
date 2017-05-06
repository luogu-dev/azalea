using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Reflection;
using Azalea.Roles.Host;

namespace Azalea.Networking
{
    public class NetHost
    {
        private const int MaxBuffer = 51200;

        private ServerDetail Detail;
        private TcpListener Listener;
        private Task AcceptClientTask;
        private bool IsRunning;

        public class HostClient {
            private TcpClient connection;
            public TcpClient Connection => connection;
            public IPEndPoint EndPoint
            {
                get
                {
                    return (IPEndPoint)Connection.Client.RemoteEndPoint;
                }
            }

            private ClientHandler handler;
            public ClientHandler Handler => handler;

            private bool IsRunning = true;

            public HostClient(TcpClient Conn)
            {
                connection = Conn;
                handler = new ClientHandler(this);
            }

            public async Task CommandClient(NetCommand<ClientCommandType> command)
            {
                var payload = command.Serialize();
                var buffer = Encoding.UTF8.GetBytes(payload);
                await connection.GetStream().WriteAsync(buffer, 0, buffer.Length);
            }

            public Task InvokeCommand(NetCommand<HostCommandType> command)
            {
                var method = typeof(ClientHandler).GetMethod(command.CommandString);
                return Task.Run(() => method.Invoke(handler, command.Parameters));
            }

			public async Task ServeClient()
			{
				var ReadBuffer = new Byte[MaxBuffer];
				while (IsRunning)
				{
                    await Connection.GetStream().ReadAsync(ReadBuffer, 0, MaxBuffer);
					var commandJson = Encoding.UTF8.GetString(ReadBuffer);
                    NetCommand<HostCommandType> command;
					try
					{
						command = NetCommand<HostCommandType>.Unserialize(commandJson);
					}
					catch (ArgumentException)
					{
						// Invalid Command
					}
                    var invoke = InvokeCommand(command);
				}
			}
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
                var clientConn = await Listener.AcceptTcpClientAsync();
                var client = new HostClient(clientConn);
                var task = client.ServeClient();
            }
        }
    }
}
