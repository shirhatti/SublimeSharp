using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SublimeSharp.Host.CodeAnalysis;
using SublimeSharp.Host.Model;

namespace SublimeSharp.Host
{
    public class SocketHost
    {
        private readonly IPEndPoint _endpoint;
        private readonly ILogger _logger;
        private readonly int _port;
        private readonly Socket _socket;
        private const int _backlogMax = 10;

        public SocketHost(int port, ILogger logger = null)
        {
            _logger = logger;
            _port = port;
            
            _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
            {
                DualMode = true
            };
            _endpoint = new IPEndPoint(IPAddress.Loopback, port);
        }

        public async void Start()
        {
            _socket.Bind(_endpoint);
            _socket.Listen(_backlogMax);

            for( ; ; )
            {
            	var acceptSocket = await AcceptAsync(_socket);

            	StartQueue(acceptSocket);
            }

       	}

   	    private static Task<Socket> AcceptAsync(Socket socket)
        {
            return Task.Factory.FromAsync((cb, state) => socket.BeginAccept(cb, state), ar => socket.EndAccept(ar), null);
        }

       	private void StartQueue(Socket socket)
       	{
            var stream = new NetworkStream(socket);
            var queue = new ProcessingQueue(stream);
            if (_logger != null)
            {
                _logger.Log("Starting socket on host {0}:{1}", _endpoint.Address, _endpoint.Port);
            }

            queue.OnReceive += m =>
            {
                if (m.MessageType == "SimpleFileCompletionMessage")
                {
                    var suggestions =
                        SimpleCompletionService.GetCompletions(m.Payload.ToObject<SimpleFileCompletionMessage>());
                    var msg = new CompletionSuggestionsMessage
                    {
                        Suggestions = suggestions
                    };

                    queue.Post(new Message
                    {
                        MessageType = "CompletionSuggestionsMessage",
                        Payload = JToken.FromObject(msg),
                    });
                }
            };

            queue.Start();
        }
    }
}