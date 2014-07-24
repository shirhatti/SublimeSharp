using System;

namespace SublimeSharp.Host
{
	public class ConsoleLogger : ILogger
	{
		public void Log(string message)
		{
			Console.WriteLine(message);
		}

		public void Log(string messageFormat, params object[] objs)
		{
			Log(String.Format(messageFormat, objs));
		}
	}
}