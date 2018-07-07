using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Kwartet.Desktop.Core;
using Kwartet.Desktop.Online;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Game = Kwartet.Desktop.Online.Game;

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
        private static Texture2D cardHiddenTexture;
        private Texture2D cardImageTexture;
        public bool Hidden { get; set; }

        private Vector2 wantedPosition;
        private Vector2 wantedSize;
        private float lerpSpeed = 18.0f;

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

            // default card to hidden
            Hidden = true;

            Size = new Vector2(0.12f);
            SetWantedSize(Size);

            if (cardHiddenTexture == null) cardHiddenTexture = Game.Content.Load<Texture2D>("card-back");
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

        public override void Draw(SpriteBatch sb)
        {
            if (Hidden) DrawHidden(sb);
            else DrawNormal(sb);
        }

        public void SetWantedPosition(Vector2 wantedPosition)
        {
            this.wantedPosition = wantedPosition;
        }

        public void SetWantedSize(Vector2 wantedSize)
        {
            this.wantedSize = wantedSize;
        }

        private void DrawHidden(SpriteBatch sb)
        {
            //sb.Draw(cardHiddenTexture, new Vector2(200), Color.White);
            sb.Draw(cardHiddenTexture, Position, null, Color.White, Rotation, Vector2.Zero, Size, SpriteEffects.None, LayerDepth);
        }

        private void DrawNormal(SpriteBatch sb)
        {
            // Draw card itself
            sb.Draw(cardTemplateTexture, Position, null, Color.White, Rotation, Vector2.Zero, Size, SpriteEffects.None,
                LayerDepth);
            
            // Draw Title (Category)
            sb.DrawString(Game1.font, ServerCard.category.ToString(), Position + new Vector2(74*Size.X,80*Size.Y), Color.Black, Rotation, Vector2.Zero, Size*4, SpriteEffects.None, LayerDepth);
            
            // Draw Image of the card
            if(cardImageTexture != null)
                sb.Draw(cardImageTexture, Position + new Vector2(74 * Size.X, 205 * Size.Y), null, Color.White, Rotation,
                    Vector2.Zero, Size, SpriteEffects.None, LayerDepth);

            // Draw the other categories
            float positionInterval = 100.0f;
            float startYPos = 900;
            int thisIndex = 0;
            
            for(int i = 0; i < ServerCard.cardsInSameCategory.Length; i++)
            {
                if (ServerCard.cardName == ServerCard.cardsInSameCategory[i]) thisIndex = i;
                
                sb.DrawString(Game1.font, ServerCard.cardsInSameCategory[i], Position + new Vector2(74 * Size.X, (startYPos + positionInterval * i)*Size.Y), Color.Black, Rotation, Vector2.Zero, Size*4, SpriteEffects.None, LayerDepth);
            }

            // Draw the current category again but with a different epic color
            sb.DrawString(Game1.font, ServerCard.cardName, Position + new Vector2(74 * Size.X, (startYPos + positionInterval * thisIndex)*Size.Y), Color.Red, Rotation, Vector2.Zero, Size*4, SpriteEffects.None, LayerDepth+1);
        }

        public void ShowOnCenter(Player ownedPlayer)
        {
            
            Task.Factory.StartNew(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(0.1)).Wait();
                
                // Set wanted position to center of screen
                Vector2 center = new Vector2(Screen.Width/2, Screen.Height/2);
                center.X -= (cardTemplateTexture.Width * Size.X) / 2;
                center.Y -= cardTemplateTexture.Height * Size.Y / 2;

                Vector2 size = Size;
                SetWantedSize(new Vector2(0.4f));
                
                SetWantedPosition(center);
                
                // Show the card.
                Hidden = false;

                Console.WriteLine("Hey");
                
                // Wait a second.
                Task.Delay(TimeSpan.FromSeconds(2)).Wait();

                Console.WriteLine("Hey");
                // Move the card back.

                SetWantedSize(size);
                Hidden = true;
                ownedPlayer.OrderCardPositions();
            });
            
        }

        public override void Update(float dt)
        {
            Position = Vector2.Lerp(Position, wantedPosition, MathHelper.Clamp(lerpSpeed, 0, 1) * dt);
            Size = Vector2.Lerp(Size, wantedSize, MathHelper.Clamp(lerpSpeed, 0, 1) * dt);
        }

        public override string ToString()
        {
            return $"{ServerCard.category.ToString()}-{ServerCard.cardName}";
        }
    }
}