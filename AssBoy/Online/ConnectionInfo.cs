using System;

namespace Kwartet.Desktop
{
    public class ConnectionInfo
    {
        public readonly string ID;
        public readonly Server Server;
        public DateTime LastHeartBeat { get; private set; }

        public ConnectionInfo(string ID, Server server)
        {
            this.ID = ID;
            this.Server = server;
            LastHeartBeat = DateTime.Now;
        }
    }
}