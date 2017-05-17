using System;
using System.Text.RegularExpressions;
using System.Reflection;
using Azalea.Networking;

namespace Azalea.Roles.Host
{
    public class ClientHandler
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

        private NetHost.HostClient HostClient;

        public ClientHandler(NetHost.HostClient hostClient)
        {
            HostClient = hostClient;
        }

        public async void Register(string _name, string _identifier)
        {
            Name = _name;
            Identifier = _identifier;

            await HostClient.CommandClient(new NetCommand<ClientCommandType>(ClientCommandType.GenericCommandSuccess, "Register"));
        }

        public void Disconnect()
        {
            HostClient.Terminate();
        }

        public void GenericCommandSuccess(string CommandName)
        {
            
        }

        public void InvalidCommand()
        {
            
        }

    }
}
