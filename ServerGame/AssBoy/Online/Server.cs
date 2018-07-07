using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Kwartet.Desktop.Online
{
    public class WebServer
    {   
        public Action<string> DataReceived;
        public Server Server { get; private set; }
        public bool Hosting { get; private set; }
        public string DisplayIPAdress;
        
        private WebSocketServer _webSocketServer;

        private List<ConnectionInfo> connectedUsers = new List<ConnectionInfo>();
        private Dictionary<ClientToServerStatus, 
            Action<ServerStatusHandler.ClientMessage>> subscriptions;

        public void StartServer()
        {
            _webSocketServer = new WebSocketServer(42069);
            _webSocketServer.AddWebSocketService("/", () =>
            {
                Server = new Server();
                Server.OnMessageReceived += Extract;
                return Server;
            });
            _webSocketServer.Start();
            Console.WriteLine("Server has been started.");

            var hostaddresses = Array.FindAll(
                Dns.GetHostEntry(string.Empty).AddressList,
                a => a.AddressFamily == AddressFamily.InterNetwork);

            DisplayIPAdress = hostaddresses[0].ToString();
            Hosting = true;
        }

        public ConnectionInfo AddUser(string ID, Server server)
        {
            var c = new ConnectionInfo(ID, server, this);
            connectedUsers.Add(c);
            return c;
        }

        public void SendToAll(IServerMessage send)
        {
            foreach (var c in connectedUsers)
            {
                c.Server.Send(send);
            }
        }

        public void SendToPlayer(string ID, IServerMessage send)
        {
            var connectedUser = connectedUsers.FirstOrDefault(x => x.ID == ID);
            connectedUser?.Server.Send(send);
        }

        private void Extract(MessageEventArgs args, Server server, string ID)
        {
            // Parse the JObject
            JObject clientMessageJObject = JObject.Parse(args.Data);
            // make a new ClientMessage
            var clientMessage = new ServerStatusHandler.ClientMessage();
            clientMessage.ID = ID;
            clientMessage.ServerStatus = server;
            
            // Parse the status 
            ClientToServerStatus status;
            if (!Enum.TryParse(clientMessageJObject["status"].ToString(), true, out status))
                status = ClientToServerStatus.Unknown;
            
            // Get the data object
            clientMessage.Data = clientMessageJObject["data"];

            var action = subscriptions.FirstOrDefault(x => x.Key == status).Value;
            action?.Invoke(clientMessage);
        }

        public void Subscribe(ClientToServerStatus status, Action<ServerStatusHandler.ClientMessage> subAction)
        {
            if(subscriptions == null) subscriptions = new 
                Dictionary<ClientToServerStatus, Action<ServerStatusHandler.ClientMessage>>();

            Action<ServerStatusHandler.ClientMessage> action;
            if (subscriptions.TryGetValue(status, out action))
            {
                action += subAction;
            }
            else
            {
                action += subAction;
                subscriptions.Add(status, action);
            }
        }

        public void Unsubscribe(ClientToServerStatus status, Action<ServerStatusHandler.ClientMessage> subAction)
        {
            if (subscriptions == null) return;
            
            Action<ServerStatusHandler.ClientMessage> action;
            if (subscriptions.TryGetValue(status, out action))
                action -= subAction;
        }

        public void StopServer()
        {
            if(!_webSocketServer.IsListening) throw new Exception("The server is not listening, so cannot stop. You nob.");
            _webSocketServer.Stop();
            Hosting = false;
        }
    }

    public class Server : WebSocketBehavior
    {
        /// <summary>
        /// event, server, sender.
        /// </summary>
        internal Action<MessageEventArgs, Server, string> OnMessageReceived;
        
        protected override void OnMessage(MessageEventArgs e)
        {
            OnMessageReceived?.Invoke(e, this, ID);
        }

        public new void Send(byte[] data)
        {
            base.Send(data);
        }

        public new void Send(FileInfo fileInfo)
        {
            base.Send(fileInfo);
        }

        public void Send(IServerMessage message)
        {
            this.Send(message.Build());
        }

        public new void Send(string data)
        {
            base.Send(data);
        }

        public void DropConnection()
        {
            Task.Factory.StartNew(() => Context.WebSocket.Close());
        }
    }
}