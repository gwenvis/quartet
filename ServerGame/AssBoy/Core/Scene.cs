using System;
using System.Collections.Generic;
using Kwartet.Desktop.Online;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Core
{
    public abstract class Scene
    {
        protected SceneManager _sceneManager;
        protected Online.Game Game => _sceneManager.Games;
        protected WebServer WebServer => _sceneManager.WebServer;
        protected GraphicsDevice GraphicsDevice => _sceneManager.GraphicsDevice;
        protected ContentManager Content => _sceneManager.Content;
        protected SpriteBatch SpriteBatch => _sceneManager.SpriteBatch;
        
        public readonly List<Entity> EntitiesInScene = new List<Entity>();

        internal void SetSceneManager(SceneManager sceneManager)
        {
            if (_sceneManager != null) return;
            _sceneManager = sceneManager;
        }

        protected void SwitchScene(Type sceneType)
        {
            _sceneManager.SwitchScene(sceneType);
        }

        public void AddEntity(Entity entity)
        {
            EntitiesInScene.Add(entity);
        }

        public virtual void Initialize() { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime dt) { }

        public void Draw(GameTime dt)
        {
            BeforeDraw(dt);
            // TODO : Draw entities in scene
            AfterDraw(dt);
        }
        
        public virtual void BeforeDraw(GameTime dt) { }
        public virtual void AfterDraw(GameTime dt) {}
        
    }
}