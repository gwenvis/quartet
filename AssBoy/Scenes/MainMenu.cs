using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using Kwartet.Desktop.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Scenes
{
    public class MainMenu : Scene
    {
        
        public override void Initialize()
        {
            WebServer.Subscribe(ClientToServerStatus.StartGame, TryStartGame);
        }

        public void TryStartGame(ServerStatusHandler.ClientMessage message)
        {
            if (Game._playersConnected.Count < 2) return;

            WebServer.Unsubscribe(ClientToServerStatus.StartGame, TryStartGame);
            SwitchScene(typeof(CardGameScene));
        }

        public override void Draw(GameTime gameTime)
        {
            if (!WebServer.Hosting) return;

            // draw the game code.
            string gamecode = $"Join the game with\n{WebServer.DisplayIPAdress}";
            var measuredString = Game1.font.MeasureString(gamecode) / 2;
            var textPosition = new Vector2((float)Screen.Width / 2, (float)Screen.Height / 2);
            
            SpriteBatch.DrawString(Game1.font, gamecode, textPosition, Color.Black, 0, measuredString, Vector2.One,
                SpriteEffects.None, 0);
            
            // show the amount of players connected, with the names.
            SpriteBatch.DrawString(Game1.font, $"Players Connected:\n{Game._playersConnected.Count}/4\nMinimum players is 2", new Vector2(50), Color.White);

        }
    }
}