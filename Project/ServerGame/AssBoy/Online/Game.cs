using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using Kwartet.Desktop.Cards;
using Kwartet.Desktop.Core;
using Kwartet.Desktop.Online;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Online
{
    public class Game
    {
        /// <summary>
        /// Players that are connected.
        /// The first player (index 0) is the host, and can decide when the player starts. (If there's more than 2 players.)
        /// </summary>
        public List<Player> PlayersConnected = new List<Player>();
        public List<Card> CardsOnTable { get; private set; }

        private readonly Random random = new Random();
        private readonly WebServer _server;

        public static ContentManager Content { get; private set; }
        public static GraphicsDevice GraphicsDevice { get; private set; }
        
        public Game(WebServer server, ContentManager content, GraphicsDevice graphicsDevice)
        {
            Content = content;
            GraphicsDevice = graphicsDevice;
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
            CardsOnTable = CardsOnTable.OrderBy(item => random.Next()).ToList();
            
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
            if (CardsOnTable.Count == 0) return null;
            
            var card = CardsOnTable[0];
            CardsOnTable.RemoveAt(0);
            return card;
        }

        public void SortCardsOnTable()
        {
            if (CardsOnTable.Count == 0) return;

            Console.WriteLine("Sorting cards on table.");
            
            Vector2 center = new Vector2(Screen.Width / 2, Screen.Height / 2);
            center.X -= CardsOnTable[0].Width / 2;
            center.Y -= CardsOnTable[0].Height / 2;
            const float xOffset = 2.0f;

            for (int i = 0; i < CardsOnTable.Count; i++)
            {
                CardsOnTable[i].SetWantedPosition(center + new Vector2(xOffset * i, 0));
            }
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
            CardsOnTable = new List<Card>()
            {
                new Card(CardCategory.Docenten, "Jelle"),
                new Card(CardCategory.Docenten, "Ed"),
                new Card(CardCategory.Docenten, "Silvan"),
                new Card(CardCategory.Docenten, "Richard"),
                
                new Card(CardCategory.Films, "Sherlock Gnomes"),
                new Card(CardCategory.Films, "Emoji Movie"),
                new Card(CardCategory.Films, "Frozen"),
                new Card(CardCategory.Films, "Deadpool 2"),
                
                new Card(CardCategory.Plaatsen, "Toronto"),
                new Card(CardCategory.Plaatsen, "Rome"),
                new Card(CardCategory.Plaatsen, "New York"),
                new Card(CardCategory.Plaatsen, "Amsterdam"),
                
                new Card(CardCategory.Noobs, "Floyd"),
                new Card(CardCategory.Noobs, "Timo"),
                new Card(CardCategory.Noobs, "Gijs"),
                new Card(CardCategory.Noobs, "Jerry"),
                
                new Card(CardCategory.HandGebaren, "Very nice"),
                new Card(CardCategory.HandGebaren, "Fuck you"),
                new Card(CardCategory.HandGebaren, "Italiano"),
                new Card(CardCategory.HandGebaren, "Good"),
                
                new Card(CardCategory.Top10Moppen, "Klop klop"),
                new Card(CardCategory.Top10Moppen, "Dokter"),
                new Card(CardCategory.Top10Moppen, "Ook Floyd"),
                new Card(CardCategory.Top10Moppen, "Belgen"),
                
                new Card(CardCategory.Memes, "Pickle Rick"),
                new Card(CardCategory.Memes, "Do you know the wae"),
                new Card(CardCategory.Memes, "Pepe"),
                new Card(CardCategory.Memes, "Spongebob"),
                
                new Card(CardCategory.KantineOpSchool, "Lekker Hamburgertje"),
                new Card(CardCategory.KantineOpSchool, "Muffin"),
                new Card(CardCategory.KantineOpSchool, "Das volgens mij een kroket"),
                new Card(CardCategory.KantineOpSchool, "wtf"),
                
                
            };

            // set all cards that have the same category
            foreach (var category in (CardCategory[])Enum.GetValues(typeof(CardCategory)))
            {
                var cardsInCategory = CardsOnTable.Where(x => x.ServerCard.category == category).ToArray();
                foreach(var card in cardsInCategory) card.SetSameCategorySet(cardsInCategory);
            }
        }
    }
}