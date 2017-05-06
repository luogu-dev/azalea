using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Azalea.Networking
{
	public enum HostCommandType
	{
		Register,
		Disconnect,
        InvalidCommand
	}

    public enum ClientCommandType
    {
        InvalidCommand
    }

    public class NetCommand<CommandType>
    {

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
            }
        }

        private Object[] parameters;
        public Object[] Parameters
        {
            get
            {
                return parameters;
            }
            private set
            {
                parameters = value;
            }
        }

        public NetCommand(CommandType command, Object[] param) 
        {
            parameters = param;
            Command = command;
        }

		public NetCommand(string commandString, Object[] param)
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
            return JsonConvert.DeserializeObject<NetCommand<CommandType>>(CommandJson);
        }
    }
}
