using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Azalea.Networking;

namespace Azalea
{
	internal static class Program
	{
        private static CommandLineApplication app;

		static void Main(string[] args)
		{
			app = new CommandLineApplication
			{
				Name = "azalea",
				Description = "AZALEA by Luogu",
				FullName = "AZALEA"
			};

            SetupApp();

			app.Execute(args);
		}

        static int HostCommand() 
        {
            var broadcast = Broadcast.Instance;
            var dummyServer = new ServerDetail("Dummy", "001122AABBCC", "127.0.0.1", 2333);
            broadcast.StartBroadcast(dummyServer);
            while (true) { }
            return 0;
        }

		static int ClientCommand()
		{
            var broadcast = Broadcast.Instance;
            var task = broadcast.GetServer();
            task.Wait();
            var server = task.Result;
            Console.WriteLine(server.Name);
			return 0;
		}

		static int JudgerCommand()
		{

			return 0;
		}

        private static void SetupApp() 
        {
			app.HelpOption("-? | --help");
			var verbose = app.Option("-v | --verbose", "Verbose output", CommandOptionType.NoValue);
            var quiet = app.Option("-q | --quiet", "Silence output", CommandOptionType.NoValue);
			
			app.Command("host", config =>
			{
				config.Description = "Run this instance as host";
                config.HelpOption("-? | --help");
                var bind = config.Option("-l | --listen <ADDRESS>", "Listen on the specified address", CommandOptionType.SingleValue);
                var noBroadcast = config.Option("--no-broadcast <ADDRESS>", "Do not broadcast this server", CommandOptionType.NoValue);
                config.OnExecute(() => HostCommand());
			});

			app.Command("client", config =>
			{
				config.Description = "Run this instance as client";
                config.HelpOption("-? | --help");
				var host = config.Option("-h | --host <ADDRESS>", "Connect to the specified host", CommandOptionType.SingleValue);
                var noDiscovery = config.Option("--no-discovery <ADDRESS>", "Do not auto discover server", CommandOptionType.NoValue);
				config.OnExecute(() => ClientCommand());
			});

			app.Command("judger", config =>
			{
				config.Description = "Run this instance as judger";
                config.HelpOption("-? | --help");
				var host = config.Option("-h | --host <ADDRESS>", "Connect to the specified host", CommandOptionType.SingleValue);
                var noDiscovery = config.Option("--no-discovery <ADDRESS>", "Do not auto discover server", CommandOptionType.NoValue);
				config.OnExecute(() => JudgerCommand());
			});

            app.OnExecute(() => 
            {
                app.ShowHelp();
                return 0;
            });
        }
	}
}
