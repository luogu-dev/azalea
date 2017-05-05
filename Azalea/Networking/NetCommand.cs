using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Azalea.Networking
{
	public enum HostCommandType
	{
		Register,
		Disconnect
	}

    public enum ClientCommandType
    {
        
    }

    public class NetCommand<CommandType>
    {
        const string CommandDetailIndex = "BuiltIn__CommandString";

        private CommandType Command;
        public string CommandString
        {
            get
            {
                return Command.ToString();
            }
            private set
            {
                Command = (CommandType)Enum.Parse(typeof(CommandType), value);
                parameters[CommandDetailIndex] = value;
            }
        }

        private Dictionary<string, string> parameters;
        public Dictionary<string, string> Parameters
        {
            get
            {
                return parameters;
            }
            private set
            {
                parameters = value;
                parameters[CommandDetailIndex] = Command.ToString();
            }
        }

        public NetCommand(CommandType command, Dictionary<string, string> param) 
        {
            parameters = param;
            Command = command;
        }

		public NetCommand(string commandString, Dictionary<string, string> param)
		{
			parameters = param;
			CommandString = commandString;
		}

        public string Serialize()
        {
            return JsonConvert.SerializeObject(Parameters);
        }

        public static NetCommand<CommandType> Unserialize(string CommandJson)
        {
            var CommandDetail = JsonConvert.DeserializeObject<Dictionary<string, string>>(CommandJson);
            if (!CommandDetail.ContainsKey(CommandDetailIndex))
                throw new ArgumentException(CommandDetailIndex + " not found in CommandJson");

            return new NetCommand<CommandType>(CommandDetail[CommandDetailIndex], CommandDetail);
        }
    }
}
