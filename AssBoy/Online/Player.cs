using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Kwartet.Desktop.Cards;

namespace Kwartet.Desktop.Online
{
    public class Player
    {
        public readonly ConnectionInfo ConnectionInfo;
        public string Name { get; private set; }
        public bool IsHost { get; }
        public readonly List<Card> CardsInHand = new List<Card>();
        
        public int Quartets { get; private set; }
        

        public Player(ConnectionInfo ci, string name, bool isHost = false)
        {
            ConnectionInfo = ci;
            IsHost = isHost;
            Name = name;
        }
        
        public void Update()
        {
            
        }

        public void AddCard(Card card)
        {
            var cards = new Card[]
            {
                card
            };

            AddCards(cards);
        }

        public void AddCards(IEnumerable<Card> cards)
        {
            var enumerable = cards as Card[] ?? cards.ToArray();
            foreach (var card in enumerable)
            {
                CardsInHand.Add(card);
            }
            
            // Before announcing, check if the new cards from a quartet.
            ProcessQuartets();
            
            // TODO : announce to the player all the new cards that were added.

        }

        public void RemoveCard(Card card)
        {
            RemoveCards(new Card[] {card});
            
        }

        public void RemoveCards(IEnumerable<Card> cards)
        {
            var enumerable = cards as Card[] ?? cards.ToArray();
            foreach (var card in enumerable)
            {
                int index = CardsInHand.FindIndex(x => x.ToString() == card.ToString());
                CardsInHand.RemoveAt(index);
            }
            
            // TODO : announce the removal of these cards.
        }

        private void ProcessQuartets()
        {
            // Iterate through all the categories and see if the player has a quartet.
            
            foreach (var category in (CardCategory[]) Enum.GetValues(typeof(CardCategory)))
            {
                if (!QuartetCategory(category)) continue;
                
                // get all cards from this category
                var cards = CardsInHand.Where(x => x.ServerCard.category == category);
                RemoveCards(cards); // remove these cards.

                Quartets++; // This guy has a new quartet! Awesome! 
            }
        }

        private bool QuartetCategory(CardCategory category)
        {
            return CardsInHand.Count(x => x.ServerCard.category == category) == 4;
        }
    }
}