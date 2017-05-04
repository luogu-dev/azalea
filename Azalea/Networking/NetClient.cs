using System;
using System.Threading.Tasks;

namespace Azalea.Networking
{
    public class NetClient
    {
        public NetClient()
        {
        }

        public async Task<bool> CheckServerHeartbeat()
		{
            // Use Broadcast signals
            return true;
		}
    }
}
