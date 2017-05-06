using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Azalea.Networking
{
	public class ServerDetail
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
					throw new FormatException("Invalid Name");
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

		public IPEndPoint EndPoint;
        public IPAddress IPAddress
		{
			get
			{
				return EndPoint.Address;
			}
			private set
			{
                EndPoint.Address = value;
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

		public ServerDetail(string _name, string _identifier, IPEndPoint _endPoint)
		{
			Name = _name;
			Identifier = _identifier;
			EndPoint = _endPoint;
		}

		public ServerDetail(string _name, string _identifier, string ipAddress, int port) :
		this(_name, _identifier, new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port))
		{
		}

		public string Serialize()
		{
            var stringArray = new List<String> { Name, Identifier, IPAddress.ToString(), Port.ToString() };
			return String.Join(";", stringArray);
		}

		public static ServerDetail Unserialize(string data)
		{
			var stringArray = data.Split(';');
			if (stringArray.Length != 4)
			{
                throw new FormatException("Invalid ServerDetail");
			}

			int port;
			if (!Int32.TryParse(stringArray[3], out port))
			{
				throw new FormatException("Port in ServerDetail is not a valid Int");
			}

            return new ServerDetail(stringArray[0], stringArray[1], stringArray[2], port);
		}
	}
}
