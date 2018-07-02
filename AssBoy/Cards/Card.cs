using System;
using Kwartet.Desktop.Core;
using Kwartet.Desktop.Online;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kwartet.Desktop.Cards
{
    /// <summary>
    /// Information of the card.
    /// </summary>
    public struct ServerCard
    {
        [JsonProperty("cardname")]
        public string cardName;
        [JsonConverter(typeof(StringEnumConverter)), JsonProperty("category")]
        public CardCategory category;
        [JsonProperty("cardsinsamecategory")]
        public string[] cardsInSameCategory;
    }
    
    public class Card : Entity
    {
        public ServerCard ServerCard;
        private static Texture2D cardTemplateTexture;
        private Texture2D cardImageTexture;

        public Card(CardCategory category, string name)
        {
            ServerCard = new ServerCard
            {
                cardName = name,
                category = category
            };

            string spritename = string.Join("-", name.ToLower().Split(' '));
            try
            {
                cardImageTexture = Online.Game.Content.Load<Texture2D>(spritename);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Card :: {spritename} could not be loaded.");
            }
            
            if (cardTemplateTexture == null) cardTemplateTexture = Game.Content.Load<Texture2D>("card");
        }

        public void SetSameCategorySet(Card[] cards)
        {
            var c = new string[cards.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                c[i] = cards[i].ServerCard.cardName;
            }

            ServerCard.cardsInSameCategory = c;
        }

        public override string ToString()
        {
            return $"{ServerCard.category.ToString()}-{ServerCard.cardName}";
        }
    }
}