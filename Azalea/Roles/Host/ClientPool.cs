using System;
using System.Collections.Generic;
using Azalea.Networking;

namespace Azalea.Roles.Host
{
    public class ClientPool
    {
        private List<NetHost.HostClient> pool;
        public List<NetHost.HostClient> Pool => pool;

        public ClientPool()
        {
        }

        public bool AddClient(NetHost.HostClient client)
        {
            if (!Pool.Contains(client))
            {
                Pool.Add(client);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void BroadcastInvoke(NetCommand<ClientCommandType> command)
        {
            foreach (var client in Pool)
            {
                var commandInvoke = client.CommandClient(command);
            }
        }

        public bool RemoveClient(NetHost.HostClient client)
        {
			if (Pool.Contains(client))
			{
                Pool.Remove(client);
				return true;
			}
			else
			{
				return false;
			}
        }
    }
}
