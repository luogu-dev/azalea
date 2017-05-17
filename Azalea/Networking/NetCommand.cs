using System;
using Newtonsoft.Json;

namespace Azalea.Networking
{
	public enum HostCommandType
	{
		Register,
		Disconnect,
        GenericCommandSuccess,
        InvalidCommand
	}

    public enum ClientCommandType
    {
        GenericCommandSuccess,
        InvalidCommand
    }

    public enum JudgerCommandType
    {
		GenericCommandSuccess,
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

		public NetCommand(CommandType command, params Object[] param)
		{
			parameters = param;
			Command = command;
		}

		private NetCommand(string commandString, Object[] param)
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
