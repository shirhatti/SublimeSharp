using System;

namespace SublimeSharp.Host
{
	public class Program
	{
		public static void Main(string[] args)
		{
			int port;
			int.TryParse(args[0], out port);

			var consoleLogger = new ConsoleLogger();
			var host = new SocketHost(port, consoleLogger);
			host.Start();
			consoleLogger.Log("Started");
			while(true);
		}
	}
}