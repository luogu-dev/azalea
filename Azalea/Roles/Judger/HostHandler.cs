using System;
using Azalea.Networking;

namespace Azalea.Roles.Judger
{
	public class HostHandler
	{
		private ServerDetail Detail;

		public HostHandler(ServerDetail detail)
		{
			Detail = detail;
		}

		public void GenericCommandSuccess(string CommandName)
		{

		}

		public void InvalidCommand()
		{

		}
	}
}
