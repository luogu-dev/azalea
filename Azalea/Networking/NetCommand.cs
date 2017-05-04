using System;
namespace Azalea.Networking
{
    public class NetCommand
    {
        public enum CommandType {
             Register,
             Disconnect
        }

        public CommandType Command;
        public string CommandString
        {
            get
            {
                return Command.ToString();
            }
            set
            {
                Command = (CommandType)Enum.Parse(typeof(CommandType), value);
            }
        }

        public NetCommand()
        {
        }
    }
}
