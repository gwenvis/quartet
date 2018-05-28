using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Kwartet.Desktop.Cards;

namespace Kwartet.Desktop
{
    public class Player
    {
        public string Name { get; private set; }
        private readonly string ID;
        public bool IsHost { get; }
        private List<Card> _cardsInHand = new List<Card>();
        private Server _connectedServer;

        public Player(Server server, string name, string ID, bool isHost = false)
        {
            _connectedServer = server;
            IsHost = isHost;
            Name = name;
            this.ID = ID;
        }
        
        public void Update()
        {
            if (CheckIfQuartet())
            {
                QuartetCategory(0);
            }
        }

        public void AddCard(Card card)
        {
            _cardsInHand.Add(card);
            
            // announce to the player that this card was just added!

            var cards = new Card[]
            {
                card
            };
            
            string json = JsonBuilder.Create()
                .WithParam("type", "newcard")
                .WithParam("cards", cards)
                .ToString();

            var bytes = Encoding.Unicode.GetBytes(json);
        }

        public void AddCards(IEnumerable<Card> cards)
        {
            var enumerable = cards as Card[] ?? cards.ToArray();
            foreach (var card in enumerable)
            {
                _cardsInHand.Add(card);
            }
            
            // announce to the player all the new cards that were added.

            string json = JsonBuilder.Create()
                .WithParam("type", "newcard")
                .WithParam("cards", enumerable)
                .ToString();

            byte[] bytes = Encoding.Unicode.GetBytes(json);
        }

        private bool CheckIfQuartet()
        {
            throw new NotImplementedException();
        }

        private bool QuartetCategory(CardCategory category)
        {
            throw new NotImplementedException();            
        }
    }

    struct MyStruct
    {
        
    }
}