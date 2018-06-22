using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Core
{
    public abstract class Scene
    {
        protected SceneManager _sceneManager;
        protected Kwartet.Desktop.Game Game => _sceneManager.Games;
        protected WebServer WebServer => _sceneManager.WebServer;
        protected GraphicsDevice GraphicsDevice => _sceneManager.GraphicsDevice;
        protected ContentManager Content => _sceneManager.Content;
        protected SpriteBatch SpriteBatch => _sceneManager.SpriteBatch;

        internal void SetSceneManager(SceneManager sceneManager)
        {
            if (_sceneManager != null) return;
            _sceneManager = sceneManager;
        }

        protected void SwitchScene(Type sceneType)
        {
            _sceneManager.SwitchScene(sceneType);
        }

        public virtual void Initialize() { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime dt) { }
        public virtual void Draw(GameTime dt) { }
    }
}