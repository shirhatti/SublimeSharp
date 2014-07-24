using System;
using System.Net;
using System.Net.Sockets;
using SublimeSharp.Host.Model;
using SublimeSharp.Host.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace SublimeSharp.Host
{
	public class SocketHost
	{
		private ILogger _logger;
		private Socket _socket;
		private IPEndPoint _endpoint;
		private readonly int _port;

		public SocketHost(int port, ILogger logger = null)
		{
			_logger = logger;
			_port = port;
			_socket = new Socket(AddressFamily.InterNetworkV6,SocketType.Stream, ProtocolType.Tcp)
						{
							DualMode = true
						};
			_endpoint = new IPEndPoint(IPAddress.Loopback, port);
		}

		public void Start()
		{
			_socket.Connect(_endpoint);
			var stream = new NetworkStream(_socket);
			var queue = new ProcessingQueue(stream);
			if(_logger != null)
			{
				_logger.Log("Starting socket on host {0}:{1}", _endpoint.Address, _endpoint.Port);
			}

			queue.OnReceive += m =>
			{
				if(m.MessageType == "SimpleFileCompletionMessage")
				{
					var suggestions = SimpleCompletionService.GetCompletions(m.Payload.ToObject<SimpleFileCompletionMessage>());
					var msg = new CompletionSuggestionsMessage
					{
						Suggestions = suggestions
					};

					queue.Post(new Message{
							MessageType = "CompletionSuggestionsMessage",
							Payload = JToken.FromObject(msg),
						});
				}
			};

			queue.Start();
		}

	}
}