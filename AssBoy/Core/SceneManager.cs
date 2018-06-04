using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Core
{
    public class SceneManager
    {
        public Scene CurrentScene { get; private set; }
        
        internal Game Games { get; }
        internal ContentManager Content { get; }
        internal GraphicsDevice GraphicsDevice { get; }
        internal SpriteBatch SpriteBatch { get; }
        internal WebServer WebServer { get; }
        
        public SceneManager(ContentManager content, 
            GraphicsDevice graphicsDevice, 
            SpriteBatch spriteBatch, 
            Game game,
            WebServer Server)
        {
            Content = content;
            GraphicsDevice = graphicsDevice;
            SpriteBatch = spriteBatch;
            Games = game;
            WebServer = Server;
        }

        public void SwitchScene(Type type)
        {
            if (!type.IsSubclassOf(typeof(Scene))) return;

            var scene = (Scene) Activator.CreateInstance(type, this);
            
            CurrentScene = (Scene)scene;
            CurrentScene?.Initialize();
            CurrentScene?.LoadContent();
        }
        
        public void Update(GameTime gameTime)
        {
            CurrentScene?.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            CurrentScene?.Draw(gameTime);
        }
    }
}