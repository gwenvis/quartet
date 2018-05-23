using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using AssBoy.Desktop.Cards;
using System.Security.Cryptography;

namespace AssBoy.Desktop
{
    public class Game
    {
        private List<Player> _playersConnected;
        private List<Card> _cardsOnTable;
        
        public Game()
        {
            MakeCards();
        }

        public void Start()
        {
            
        }

        private void MakeCards()
        {
            
        }
    }
}