using System;
using System.Collections.Generic;
using AssBoy.Desktop.Cards;

namespace AssBoy.Desktop
{
    public class Player
    {
        private string name;
        private List<Card> _cardsInHand = new List<Card>();

        public void Update()
        {
            if (CheckIfQuartet())
            {
                QuartetCategory(CardCategory.Diseases);
            }
        }

        public void AddCard(Card card)
        {
            _cardsInHand.Add(card);
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