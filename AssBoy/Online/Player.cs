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
        public readonly ConnectionInfo ConnectionInfo;
        public string Name { get; private set; }
        public bool IsHost { get; }
        private List<Card> _cardsInHand = new List<Card>();
        

        public Player(ConnectionInfo ci, string name, bool isHost = false)
        {
            ConnectionInfo = ci;
            IsHost = isHost;
            Name = name;
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
            
            // TODO : announce to the player all the new cards that were added.
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
}