using System.Collections.Generic;
using Kwartet.Desktop.Cards;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Kwartet.Desktop
{
    public class ServerStatusHandler
    {   
        /// <summary>
        /// Extracted message sent from a client.
        /// </summary>
        public struct ClientMessage
        {
            public Server ServerStatus { get; set; }
            public JToken Data { get; set; }
            public string ID { get; set; }
        }

        public interface IServerMessage
        {
            string Build();
        }
        
        /// <summary>
        /// Message data that the server will send.
        /// </summary>
        public struct ServerMessage<T> : IServerMessage where T : struct
        {
            [JsonProperty("status"),
            JsonConverter(typeof(StringEnumConverter))]
            public ServerToClientStatuses status;
            [JsonProperty("data")]
            public T data;
            
            public ServerMessage(ServerToClientStatuses status, T data)
            {
                this.status = status;
                this.data = data;
            }

            public string Build()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        /// <summary>
        /// Sent to a player that has joined, granting them an ID. 
        /// </summary>
        public struct JoinInfo
        {
            [JsonProperty("id")]
            private int id;

            public JoinInfo(int id)
            {
                this.id = id;
            }
        }

        /// <summary>
        /// Sent to a player that receives new cards.
        /// </summary>
        public struct CardsReceiveInfo
        {
            private ServerCard[] cards;
            
            public CardsReceiveInfo(ServerCard[] cardslist)
            {
                cards = cardslist;
            }
        }

        /// <summary>
        /// Sent to a player whose turn just started,
        /// this contains the information with all players connected. (Even the player self)
        /// this allows him to select a list of names
        /// </summary>
        public struct TurnStartedInfo
        {
            // information for the client
            struct ClientPlayer
            {
                public string PlayerName;
                public string ID;
            }

            private ClientPlayer[] players;

            public TurnStartedInfo(Player[] players)
            {
                var clientPlayers = new List<ClientPlayer>();

                foreach (var player in players)
                {
                    clientPlayers.Add(new ClientPlayer()
                    {
                        PlayerName = player.Name,
                        ID = player.ConnectionInfo.ID
                    });
                }

                this.players = clientPlayers.ToArray();
            }
        }
        
        public struct EmptyInfo { }
    }

    /// <summary>
    /// Messages from the client to the server.
    /// </summary>
    public enum ClientToServerStatus
    {
        /// <summary>
        /// Someone joins the game.
        /// </summary>
        Join,
        
        /// <summary>
        /// Someone disconnects from the game.
        /// </summary>
        Disconnect,
        
        /// <summary>
        /// The host requests to start the game.
        /// </summary>
        StartGame,
        
        /// <summary>
        /// A client asks for a card.
        /// </summary>
        Question,
        
        /// <summary>
        /// Client requests that are unknown. They are
        /// </summary>
        Unknown
    }
    
    /// <summary>
    /// Messages from the server to the client
    /// </summary>
    public enum ServerToClientStatuses
    {
        /// <summary>
        /// Send to all players, included with a name of the player that won.
        /// If it's a tie, it will send the same, but instead with the players that have the
        /// same amount of quartets.
        /// </summary>
        Win,
        
        /// <summary>
        /// Send to a player when he receives a card. (Only that player)
        /// </summary>
        GetCard,
        
        /// <summary>
        /// When a player gives a card to another. (This will be sent in combination with GetCard.)
        /// </summary>
        GiveCard,
        
        /// <summary>
        /// When a player has a quartet, automatically remove all the cards in that category.
        /// The quartet will be displayed on the game screen.
        /// </summary>
        GotQuartet,
        
        /// <summary>
        /// When the turn has ended, send this to the player whos turn has ended.
        /// This will be send in combination with TurnStarted. Not at the same time, but in order.
        /// </summary>
        TurnEnded,
        
        /// <summary>
        /// When the player's turn has started, send this to the player whose turn has started.
        /// </summary>
        TurnStarted,
        
        /// <summary>
        /// Send this to the player when the connection has dropped.
        /// </summary>
        DropConnection,
        
        /// <summary>
        /// Send this to a player to give them their ID
        /// </summary>
        JoinInfo,
        
        /// <summary>
        /// Send this to all players when a game starts.
        /// </summary>
        StartGame,
        
        /// <summary>
        /// Unknown data.
        /// </summary>
        Unknown
        
    }
}