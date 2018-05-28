using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Kwartet.Desktop
{
    public class ServerStatusHandler
    {
        public enum ServerStatuses
        {
            Join,
            Disconnect,
            Question,
            Unknown
        }
        
        public struct ClientMessage
        {
            public Server Status { get; set; }
            public JToken Data { get; set; }
            public string ID { get; set; }
        }
    }
}