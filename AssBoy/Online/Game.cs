using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using Kwartet.Desktop.Cards;
using Kwartet.Desktop.Online;
using Microsoft.Xna.Framework.Content;

namespace Kwartet.Desktop.Online
{
    public class Game
    {
        /// <summary>
        /// Players that are connected.
        /// The first player (index 0) is the host, and can decide when the player starts. (If there's more than 2 players.)
        /// </summary>
        public List<Player> PlayersConnected = new List<Player>();
        private List<Card> _cardsOnTable;

        private readonly Random random = new Random();
        private readonly WebServer _server;

        public static ContentManager Content { get; private set; }
        
        public Game(WebServer server, ContentManager content)
        {
            Content = content;
            _server = server;
            MakeCards();
        }

        public void Start()
        {
            HandOutCards();
            _server.StartServer();
        }

        public void HandOutCards()
        {
            const int cardsToGiveOut = 5; // Everyone gets this certain amount of cards.
            
            // randomize the list
            _cardsOnTable = _cardsOnTable.OrderBy(item => random.Next()).ToList();
            
            foreach (var player in PlayersConnected)
            {
                Card[] cards = new Card[cardsToGiveOut];
                
                for (int i = 0; i < cardsToGiveOut; i++)
                {
                    cards[i] = PopCard();
                }

                player.AddCards(cards);
            }
        }

        public Card PopCard()
        {
            if (_cardsOnTable.Count == 0) return null;
            
            var card = _cardsOnTable[0];
            _cardsOnTable.RemoveAt(0);
            return card;
        }
        

        public Player GetPlayer(string ID)
        {
            return PlayersConnected.FirstOrDefault(x => x.ConnectionInfo.ID == ID);
        }
        
        public int PlayerJoin(Player player)
        {
            PlayersConnected.Add(player);
            return PlayersConnected.Count;
        }
        
        private void MakeCards()
        {
            _cardsOnTable = new List<Card>()
            {
                new Card(CardCategory.Teachers, "Jelle"),
                new Card(CardCategory.Teachers, "Ed"),
                new Card(CardCategory.Teachers, "Silvan"),
                new Card(CardCategory.Teachers, "Richard"),
                
                new Card(CardCategory.Movies, "Sherlock Gnomes"),
                new Card(CardCategory.Movies, "Emoji Movie"),
                new Card(CardCategory.Movies, "Frozen"),
                new Card(CardCategory.Movies, "Deadpool 2"),
                
                new Card(CardCategory.Places, "Toronto"),
                new Card(CardCategory.Places, "Rome"),
                new Card(CardCategory.Places, "New York"),
                new Card(CardCategory.Places, "Amsterdam"),
                
                new Card(CardCategory.Students, "Floyd"),
                new Card(CardCategory.Students, "Timo"),
                new Card(CardCategory.Students, "Gijs"),
                new Card(CardCategory.Students, "Jerry"),
                
                new Card(CardCategory.Gestures, "Very nice"),
                new Card(CardCategory.Gestures, "Fuck you"),
                new Card(CardCategory.Gestures, "Italiano"),
                new Card(CardCategory.Gestures, "Good"),
            };

            // set all cards that have the same category
            foreach (var category in (CardCategory[])Enum.GetValues(typeof(CardCategory)))
            {
                var cardsInCategory = _cardsOnTable.Where(x => x.ServerCard.category == category).ToArray();
                foreach(var card in cardsInCategory) card.SetSameCategorySet(cardsInCategory);
            }
        }
    }
}