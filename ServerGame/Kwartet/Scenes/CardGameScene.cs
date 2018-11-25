using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        private const float questionDisplayTime = 10.0f;
        private float currentDisplayTime = questionDisplayTime +1;
        private string question = "";
        private Player.Corner askedCorner;

        private string response = "";
        private Player.Corner responseCorner;
        
        public static readonly Dictionary<Player.Corner, Vector2> CornerPositions = new Dictionary<Player.Corner, Vector2>()
        {
            {Player.Corner.UpLeft, new Vector2(100,50)},
            {Player.Corner.UpRight, new Vector2(Screen.Width - 200, 50)},
            {Player.Corner.DownLeft, new Vector2(100, Screen.Height - 250)},
            {Player.Corner.DownRight, new Vector2(Screen.Width - 200, Screen.Height - 250)},
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
            
            Game.SortCardsOnTable();
            
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

            currentDisplayTime = 0;
            askedCorner = player.PlayerCorner;
            responseCorner = askedPlayer.PlayerCorner;
            question = $"{askedPlayer.Name},\nHeb jij van {processedCard.category},\n{processedCard.name}?";
            
            bool foundCard = card != null;

            if (foundCard)
            {
                askedPlayer.RemoveCard(card);
                player.AddCard(card);
                
                card.ShowOnCenter(player);

                response = "yep verdomme man";
                
                AnnouncePlayerTurnStart();
            }
            else
            {
                // if the card has NOT been found, just start the next turn I guess? OH! And give the player a card!

                var poppedCard = Game.PopCard();
                if (poppedCard != null) player.AddCard(poppedCard);
                Game.SortCardsOnTable(); // sort the cards on the table.

                response = "haha nee";
                
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
            // Before announcing the player turn start, check if the player even has any cards left
            // If he does not, give him a card and end his turn. If there are no remaining cards left on the table,
            // he's out and will just completely be skipped as life dictates.

            var player = Game.PlayersConnected[currentPlayerIndex];
            
            if (player.CardsInHand.Count == 0)
            {
                var popCard = Game.PopCard();
                if (popCard != null) player.AddCard(popCard);
                StartNextTurn();
                return; // noob! No new turn for you.
            }
            
            string id = player.ConnectionInfo.ID;
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

        public override void BeforeUpdate(GameTime dt)
        {
            foreach (var player in Game.PlayersConnected)
            {
                foreach (var card in player.CardsInHand)
                {
                    card.Update((float)dt.ElapsedGameTime.TotalSeconds);
                }
            }

            foreach (var carad in Game.CardsOnTable)
            {
                carad.Update((float)dt.ElapsedGameTime.TotalSeconds);
            }

            currentDisplayTime += (float) dt.ElapsedGameTime.TotalSeconds;
        }

        public override void BeforeDraw(GameTime dt)
        {
            // Draw all cards on the table.

            for (int i = Game.CardsOnTable.Count - 1; i >= 0; i--) 
            {
                Game.CardsOnTable[i].Draw(SpriteBatch);
            }   
            
            // draw all player cards, name and their kwartet amounts
            // in their respective corners
            foreach (var player in Game.PlayersConnected)
            {                
                foreach (var card in player.CardsInHand)
                {
                    card.Draw(SpriteBatch);
                }
                
                bool below = player.PlayerCorner.ToString().StartsWith("Down");
                Vector2 cornerPosition = CornerPositions[player.PlayerCorner];

                cornerPosition.Y += below ? (200) : -40;

                SpriteBatch.DrawString(Game1.font, player.Name, cornerPosition, Color.Black);

                cornerPosition = CornerPositions[player.PlayerCorner];
                cornerPosition.Y -= below ? (40) : -200;
                
                SpriteBatch.DrawString(Game1.font, $"Kwartetten: {player.Quartets}", cornerPosition, Color.Black);
            }
            
            // last but least, draw the question if needed.
            if (currentDisplayTime <= questionDisplayTime)
            {
                Vector2 GetCornerPos(Player.Corner corner)
                {
                    Vector2 cornerPosition = CornerPositions[corner];
                    string s = corner.ToString().ToLower();

                    if (s.Contains("left")) cornerPosition.X += 200;
                    else cornerPosition.X -= 400;

                    return cornerPosition;
                }

                var c = GetCornerPos(askedCorner);
                var b = GetCornerPos(responseCorner);
                
                SpriteBatch.DrawString(Game1.font, question, c, Color.Black);
                SpriteBatch.DrawString(Game1.font, response, b, Color.Black);
            }
        }
    }
}