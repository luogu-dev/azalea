using System;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Reflection;
using Azalea.Networking;

namespace Azalea.Roles.Client
{
    public class NetClient
    {
        private const int MaxBuffer = 51200;

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            private set
            {
                if (value.Length == 0 || value.Length > 20)
                {
                    throw new ArgumentException("Name must be between 1-20 chars long");
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
                    throw new FormatException("Invalid Identifier");
                }

                identifier = value;
            }
        }

        private ServerDetail HostDetail;
        private TcpClient Connection;
        private HostHandler handler;
        public HostHandler Handler => handler;

        private bool IsRunning;

        private static NetClient instance;
        public static NetClient Instance => instance;

        public NetClient(ServerDetail hostDetail, string _name, string _identifier)
        {
            instance = this;
            HostDetail = hostDetail;
            Name = _name;
            Identifier = _identifier;
        }

        public async Task<TcpClient> Connect()
        {
            Connection = new TcpClient();
            await Connection.ConnectAsync(HostDetail.EndPoint.Address, HostDetail.EndPoint.Port);

            await CommandHost(new NetCommand<HostCommandType>(HostCommandType.Register, Name, Identifier));
            var hostComm = HostCommunication();

            handler = new HostHandler();

            IsRunning = true;
            return Connection;
        }

        private async Task HostCommunication()
        {
            var ReadBuffer = new Byte[MaxBuffer];
            while (IsRunning)
            {
                await Connection.GetStream().ReadAsync(ReadBuffer, 0, MaxBuffer);
                var commandJson = Encoding.UTF8.GetString(ReadBuffer);

                NetCommand<ClientCommandType> command;
                try
                {
                    command = NetCommand<ClientCommandType>.Unserialize(commandJson);
                }
                catch (ArgumentException)
                {
                    _ = CommandHost(new NetCommand<HostCommandType>(HostCommandType.InvalidCommand));
                    continue;
                }

                _ = InvokeCommand(command);
            }
        }

        public void Terminate()
        {
            IsRunning = false;
        }

        public async Task CommandHost(NetCommand<HostCommandType> command)
        {
            var payload = command.Serialize();
            var buffer = Encoding.UTF8.GetBytes(payload);
            await Connection.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        private Task InvokeCommand(NetCommand<ClientCommandType> command)
        {
            var method = typeof(HostHandler).GetMethod(command.CommandString);
            return Task.Run(() => method.Invoke(Handler, command.Parameters));
        }
    }
}
