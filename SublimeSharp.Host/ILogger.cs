using System;

namespace SublimeSharp.Host
{
	public interface ILogger
	{
		void Log(string message);
		void Log(string messageFormat, params object[] pieces);

	}
}