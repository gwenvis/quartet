using System;

namespace Kwartet.Desktop.Online
{
    public class ConnectionInfo
    {
        public readonly string ID;
        public readonly Server Server;
        public readonly WebServer WebServer;
        public DateTime LastHeartBeat { get; private set; }

        public ConnectionInfo(string ID, Server server, WebServer webserver)
        {
            this.ID = ID;
            this.Server = server;
            LastHeartBeat = DateTime.Now;
            WebServer = webserver;
        }
    }
}