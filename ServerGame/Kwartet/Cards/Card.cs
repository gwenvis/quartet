using System;
using System.Net;
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

        public float Width => cardTemplateTexture.Width * Size.X;
        public float Height => cardTemplateTexture.Height * Size.Y;

        private Vector2 wantedPosition;
        private Vector2 wantedSize;
        private float lerpSpeed = 5.0f;

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
                var stream = System.IO.File.OpenRead("dataimg/" + spritename + ".png");

                cardImageTexture = Texture2D.FromStream(Game.GraphicsDevice, stream);

                //cardImageTexture = Online.Game.Content.Load<Texture2D>(spritename);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Card :: {spritename} could not be loaded.");
            }

            // default card to hidden
            Hidden = true;

            Size = new Vector2(0.12f);
            SetWantedSize(Size);

            LayerDepth = 1;

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
                    Vector2.Zero, Size*2, SpriteEffects.None, LayerDepth);

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
            sb.DrawString(Game1.font, ServerCard.cardName, Position + new Vector2(74 * Size.X, (startYPos + positionInterval * thisIndex)*Size.Y), Color.Red, Rotation, Vector2.Zero, Size*4, SpriteEffects.None, LayerDepth);
        }

        public void ShowOnCenter(Player ownedPlayer)
        {
            Task.Factory.StartNew(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(0.2)).Wait();

                if (k) return;
                
                // Set wanted position to center of screen
                Vector2 center = new Vector2(Screen.Width/2, Screen.Height/2);
                center.X -= cardTemplateTexture.Width * 0.4f /2;
                center.Y -= cardTemplateTexture.Height * 0.4f /2;

                int l = LayerDepth;
                LayerDepth = 0;
                Vector2 size = Size;
                SetWantedSize(new Vector2(0.4f));
                
                SetWantedPosition(center);
                
                // Show the card.
                Hidden = false;
                
                // Wait a second, or two.
                Task.Delay(TimeSpan.FromSeconds(2)).Wait();

                // Move the card back, and don't show it anymore.

                SetWantedSize(size);
                LayerDepth = l;
                Hidden = true;
                ownedPlayer.OrderCardPositions();
                
            });
        }

        private bool k = false;
        
        public void Quartet(Player.Corner corner, Vector2 wantedPosition)
        {
            Task.Factory.StartNew(() =>
            {
                // Sorry for this stupid hack but this will take care that it's in the correct state and doesn't 
                // show the card to all the players in a big size instead, but gets it in a row!
                k = true;
                
                Hidden = false;
                lerpSpeed *= 2;
                
                SetWantedPosition(wantedPosition);

                Task.Delay(5000).Wait();

                Vector2 cornerPosition = new Vector2();
                bool right, down;
                string cornerstring = corner.ToString().ToLower();
                right = cornerstring.Contains("right");
                down = cornerstring.StartsWith("down");

                if (right) cornerPosition.X = Screen.Width;
                else cornerPosition.X = -Width;

                if (down) cornerPosition.Y = Screen.Height;
                else cornerPosition.Y = -Height;
                
                SetWantedPosition(cornerPosition);
                SetWantedSize(new Vector2(0));

                Task.Delay(4000).Wait();

                SceneManager.CurrentScene.Destroy(this);
            });
        }

        public override void Update(float dt)
        {
            Position = Vector2.Lerp(Position, wantedPosition, MathHelper.Clamp(lerpSpeed * dt, 0, 1));
            Size = Vector2.Lerp(Size, wantedSize, MathHelper.Clamp(lerpSpeed * dt * 50, 0, 1) * dt);
        }

        public override string ToString()
        {
            return $"{ServerCard.category.ToString()}-{ServerCard.cardName}";
        }
    }
}