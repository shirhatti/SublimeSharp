using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SublimeSharp.Host.Model
{
	public class Message
	{
		public string HostId { get; set; }
        public string MessageType { get; set; }
        public int ContextId { get; set; }
        public JToken Payload { get; set; }

        public override string ToString()
        {
            return "(" + HostId + ", " + MessageType + ", " + ContextId + ") -> " + (Payload == null ? "null" : Payload.ToString(Formatting.Indented));
        }
	}
}