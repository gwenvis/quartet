using System;
using Kwartet.Desktop.Online;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game = Microsoft.Xna.Framework.Game;

namespace Kwartet.Desktop.Core
{
    public class SceneManager
    {
        public static Scene CurrentScene { get; private set; }
        
        internal Online.Game Games { get; }
        internal ContentManager Content { get; }
        internal GraphicsDevice GraphicsDevice { get; }
        internal SpriteBatch SpriteBatch { get; }
        internal WebServer WebServer { get; }
        
        public SceneManager(ContentManager content, 
            GraphicsDevice graphicsDevice, 
            SpriteBatch spriteBatch, 
            Desktop.Online.Game game,
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

            var scene = (Scene) Activator.CreateInstance(type);

            CurrentScene?.UnloadContent();
            CurrentScene = (Scene)scene;
            CurrentScene.SetSceneManager(this);
            CurrentScene?.Initialize();
            CurrentScene?.LoadContent();
        }
        
        public void Update(GameTime gameTime)
        {
            CurrentScene?.BeforeUpdate(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            CurrentScene?.Draw(gameTime);
        }
    }
}