using Kwartet.Desktop.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Cards
{
    public class Card : Entity
    {
        public string CardName { get; }
        public CardCategory Category { get; }
        private static Texture2D cardTemplateTexture;
        private Texture2D cardImageTexture;

        public Card[] SameCategoryCards { get; private set; }

        public Card(CardCategory category, string name)
        {
            CardName = name;
            Category = category;

            string spritename = string.Join("-", name.ToLower().Split(' '));
            //cardImageTexture = Game.Content.Load<Texture2D>(spritename);
            //if (cardTemplateTexture == null) cardTemplateTexture = Game.Content.Load<Texture2D>("template");
        }

        public void SetSameCategorySet(Card[] cards)
        {
            SameCategoryCards = cards;
        }

        public override void Draw(SpriteBatch sb)
        {
            
        }

        public override string ToString()
        {
            return $"{Category.ToString()}-{CardName}";
        }
    }
}