using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Kwartet.Desktop.Cards;
using Kwartet.Desktop.Scenes;
using Microsoft.Xna.Framework;

namespace Kwartet.Desktop.Online
{
    public class Player
    {
        public readonly ConnectionInfo ConnectionInfo;
        public string Name { get; private set; }
        public bool IsHost { get; }
        public readonly List<Card> CardsInHand = new List<Card>();
        public Corner PlayerCorner { get; private set; }
        
        
        public int Quartets { get; private set; }
        

        public Player(ConnectionInfo ci, string name, bool isHost = false)
        {
            ConnectionInfo = ci;
            IsHost = isHost;
            Name = name;
        }

        public void SetCorner(Corner corner)
        {
            PlayerCorner = corner;
        }

        public void AddCard(Card card, bool announce = true)
        {
            var cards = new Card[]
            {
                card
            };

            AddCards(cards, announce);
        }

        public void AddCards(IEnumerable<Card> cards, bool announce = true)
        {
            var enumerable = cards as Card[] ?? cards.ToArray();
            foreach (var card in enumerable)
            {
                CardsInHand.Add(card);
            }
            
            // Before announcing, check if the new cards from a quartet.
            if (ProcessQuartets()) announce = false;
            
            OrderCardPositions();

            if (!announce) return;
            
            // announce to the player all the new cards that were added.
            var cardList = new ServerStatusHandler.CardsReceiveInfo(enumerable.Select(x=>x.ServerCard).ToArray());
            var message =
                new ServerMessage<ServerStatusHandler.CardsReceiveInfo>(ServerToClientStatuses.GetCard, cardList);
            ConnectionInfo.Server.Send(message);
        }

        public void OrderCardPositions()
        {
            const float cardXInterval = 2.0f; // epic
            const float cardYInterval = 0.2f; // eic
            Vector2 startPos = CardGameScene.CornerPositions[PlayerCorner];
            
            for (int i = 0; i < CardsInHand.Count; i++)
            {
                var card = CardsInHand[i];
                card.SetWantedPosition(startPos + new Vector2(cardXInterval * i, cardYInterval * i));
            }
        }

        public void RemoveCard(Card card, bool announce = true)
        {
            RemoveCards(new Card[] {card}, announce);
        }

        public void RemoveCards(IEnumerable<Card> cards, bool announce = true)
        {
            var enumerable = cards as Card[] ?? cards.ToArray();
            foreach (var card in enumerable)
            {
                int index = CardsInHand.FindIndex(x => x.ToString() == card.ToString());
                CardsInHand.RemoveAt(index);
            }

            OrderCardPositions();
            
            if (!announce) return;
            
            // Announce the removal of these cards
            var cardList = new ServerStatusHandler.CardsReceiveInfo(enumerable.Select(x=>x.ServerCard).ToArray());
            var message =
                new ServerMessage<ServerStatusHandler.CardsReceiveInfo>(ServerToClientStatuses.GiveCard, cardList);
            ConnectionInfo.Server.Send(message);
            
            
        }

        private bool ProcessQuartets()
        {
            if (CardsInHand.Count == 0 || CardsInHand == null) return false;
            
            // Iterate through all the categories and see if the player has a quartet.

            bool hasQuartet = false;
            
            foreach (var category in (CardCategory[]) Enum.GetValues(typeof(CardCategory)))
            {
                if (!QuartetCategory(category)) continue;
                
                // get all cards from this category
                var cards = CardsInHand.Where(x => x.ServerCard.category == category);
                RemoveCards(cards); // remove these cards.

                Quartets++; // This guy has a new quartet! Awesome! 
                hasQuartet = true;
            }

            return hasQuartet;
        }

        private bool QuartetCategory(CardCategory category)
        {
            return CardsInHand.Count(x => x.ServerCard.category == category) == 4;
        }

        public enum Corner
        {
            UpLeft = 0,
            DownRight,
            UpRight,
            DownLeft,
        }
    }
}