using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Core
{
    public abstract class Scene
    {
        private SceneManager _sceneManager;
        
        protected Kwartet.Desktop.Game Game => _sceneManager.Games;
        protected WebServer WebServer => _sceneManager.WebServer;
        protected GraphicsDevice GraphicsDevice => _sceneManager.GraphicsDevice;
        protected ContentManager Content => _sceneManager.Content;
        protected SpriteBatch SpriteBatch => _sceneManager.SpriteBatch;

        internal Scene(SceneManager sceneManager)
        {
            SetSceneManager(sceneManager);
        }

        internal void SetSceneManager(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public abstract void Initialize();
        public abstract void LoadContent();
        public abstract void Update(GameTime dt);
        public abstract void Draw(GameTime dt);
    }
}