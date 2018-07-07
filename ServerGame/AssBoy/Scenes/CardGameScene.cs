using System;
using System.Collections.Generic;
using System.Linq;
using Kwartet.Desktop.Cards;
using Kwartet.Desktop.Core;
using Kwartet.Desktop.Online;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace Kwartet.Desktop.Scenes
{
    public class CardGameScene : Scene
    {
        private int currentPlayerIndex = 0;
        private Random random = new Random();
        
        public static readonly Dictionary<Player.Corner, Vector2> CornerPositions = new Dictionary<Player.Corner, Vector2>()
        {
            {Player.Corner.UpLeft, new Vector2(100,50)},
            {Player.Corner.UpRight, new Vector2(Screen.Width - 100, 50)},
            {Player.Corner.DownLeft, new Vector2(100, Screen.Height - 50)},
            {Player.Corner.DownRight, new Vector2(Screen.Width - 200, Screen.Height - 300)},
        };

        public override void Initialize()
        {
            // Hand out all the cards to the players.
            Game.HandOutCards();
           
            // Give all players a corner
            int corner = 0;

            foreach (var player in Game.PlayersConnected)
            {
                player.SetCorner((Player.Corner)corner);
                corner++;
                
                player.OrderCardPositions();
            }
            
            // let a random player start
            currentPlayerIndex = random.Next(Game.PlayersConnected.Count);
            AnnouncePlayerTurnStart();
            
            // subscribe to a question of a player ( asking for a card )
            WebServer.Subscribe(ClientToServerStatus.Question, OnQuestion);
        }

        private void OnQuestion(ServerStatusHandler.ClientMessage obj)
        {
            // TODO : Process the question
            var processedCard = ProcessQuestion(obj.Data);
            var player = Game.GetPlayer(obj.ID);
            
            // get the hand of that player
            var askedPlayer = Game.GetPlayer(processedCard.id);
            if(askedPlayer == null) throw new Exception("Couldn't find the player.");

            var card = askedPlayer.CardsInHand.FirstOrDefault(x =>
                x.ServerCard.cardName.ToLower() == processedCard.name.ToLower() &&
                x.ServerCard.category.ToString().ToLower() == processedCard.category.ToLower());

            bool foundCard = card != null;

            if (foundCard)
            {
                askedPlayer.RemoveCard(card);
                player.AddCard(card);
                
                card.ShowOnCenter(player);
                
                AnnouncePlayerTurnStart();
            }
            else
            {
                // if the card has NOT been found, just start the next turn I guess? OH! And give the player a card!

                var poppedCard = Game.PopCard();
                if (poppedCard != null) player.AddCard(poppedCard);
                StartNextTurn();
            }
        }

        private void StartNextTurn()
        {
            AnnouncePlayerTurnEnd();
            
            if (++currentPlayerIndex == Game.PlayersConnected.Count) currentPlayerIndex = 0;

            AnnouncePlayerTurnStart();
        }

        
        private (string id, string name, string category) 
            ProcessQuestion(JToken data)
        {
            string id = data["to"].ToString();
            string name = data["cardname"].ToString();
            string category = data["cardcategory"].ToString();
            return (id, name, category);
        }

        private void AnnouncePlayerTurnStart()
        {
            string id = Game.PlayersConnected[currentPlayerIndex].ConnectionInfo.ID;
            var info = new ServerMessage<ServerStatusHandler.TurnStartedInfo>(ServerToClientStatuses.TurnStarted,
                new ServerStatusHandler.TurnStartedInfo(Game.PlayersConnected.ToArray()));
            
            WebServer.SendToPlayer(id, info);
        }

        private void AnnouncePlayerTurnEnd()
        {
            string id = Game.PlayersConnected[currentPlayerIndex].ConnectionInfo.ID;
            
            WebServer.SendToPlayer(id, new ServerMessage<ServerStatusHandler.EmptyInfo>
                (ServerToClientStatuses.TurnEnded, new ServerStatusHandler.EmptyInfo()));
        }
        
        private void AnnounceCards()
        {
            foreach (var player in Game.PlayersConnected)
            {
                ServerCard[] cards = player.CardsInHand.Select(x => x.ServerCard).ToArray();
                var serverMessage = new ServerMessage<ServerStatusHandler.CardsReceiveInfo>
                    (ServerToClientStatuses.GetCard, new ServerStatusHandler.CardsReceiveInfo(cards));
                player.ConnectionInfo.Server.Send(serverMessage);
            }
        }

        public override void Update(GameTime dt)
        {
            foreach (var player in Game.PlayersConnected)
            {
                foreach (var card in player.CardsInHand)
                {
                    card.Update((float)dt.ElapsedGameTime.TotalSeconds);
                }
            }
        }

        public override void AfterDraw(GameTime dt)
        {
            // draw all player names in their respective corners
            foreach (var player in Game.PlayersConnected)
            {
                foreach (var card in player.CardsInHand)
                {
                    card.Draw(SpriteBatch);
                }
            }
        }
    }
}