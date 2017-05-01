using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Azalea.Exceptions;

namespace Azalea
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			var app = new CommandLineApplication
			{
				Name = "azalea",
				Description = "AZALEA by Luogu",
				FullName = "AZALEA"
			};

			app.HelpOption("-? | --help");
			var verbose = app.Option("-v | --verbose", "Verbose output", CommandOptionType.NoValue);
			var bind = app.Option("-l | --listen <ADDRESS>", "Listen on the specified address", CommandOptionType.SingleValue);
			var host = app.Option("-h | --host <ADDRESS>", "Connect to the specified host", CommandOptionType.SingleValue);
			var role = app.Argument("[role]", "Role of this instance <host|judger|client>");

			app.OnExecute(() =>
			{
				if (role.Values.Count != 1)
				{
					throw new InvalidArgumentException("Unspecified role");
				}

				var availableRoles = new List<string> { "host", "judger", "client" };
				if (!availableRoles.Contains(role.Value))
				{
					throw new InvalidArgumentException(String.Format("Invalid role {0}.", role.Value));
				}

				return 0;
			});

			try
			{
				app.Execute();
			}
			catch (InvalidArgumentException e)
			{
				app.Out.WriteLine("InvalidArgumentException: " + e.Message);
				app.ShowHelp();
			}

		}
	}
}
