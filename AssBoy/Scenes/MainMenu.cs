using System.Collections.Generic;
using System.Diagnostics;
using Kwartet.Desktop.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Scenes
{
    public class MainMenu : Scene
    {
        
        public override void Initialize()
        {
            
        }

        public override void LoadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            if (!WebServer.Hosting) return;

            // draw the game code.
            string gamecode = $"Join the game with\n{WebServer.DisplayIPAdress}";
            var measuredString = Game1.font.MeasureString(gamecode) / 2;
            var textPosition = new Vector2((float)Screen.Width / 2 - measuredString.X, (float)Screen.Height / 2 - measuredString.Y);
            
            SpriteBatch.DrawString(Game1.font, gamecode, textPosition, Color.Black, 0, Vector2.Zero, Vector2.One,
                SpriteEffects.None, 0);

            Debug.WriteLine("Hey bitch I am drawing here.");

            // show the amount of players connected, with the names.


        }

        public MainMenu(SceneManager sceneManager) : base(sceneManager)
        {
            
        }
    }
}