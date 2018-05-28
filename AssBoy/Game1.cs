using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kwartet.Desktop
{
    /// <summary>
    /// This is the mains     type for your game.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private WebServer server;
        private Desktop.Game _game;

        private SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            server = new WebServer();
            _game = new Game(server, Content);
            
            _game.Start();
            
            server.Subscribe(ServerStatusHandler.ServerStatuses.Join, (a) =>
            {
                _game.PlayerJoin(new Player(server.Server, a.Data["name"].ToString(), a.ID));
                Console.WriteLine(a.Data["name"].ToString());
            });
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("spritefont");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.YellowGreen);

            spriteBatch.Begin();
            string names = "";
            _game._playersConnected.ForEach(x=>names += x.Name + "\n");
            spriteBatch.DrawString(font, names, new Vector2(2),Color.Black);
            if(server.Hosting)
                spriteBatch.DrawString(font,
                    server.DisplayIPAdress,
                    new Vector2((float)GraphicsDevice.Viewport.Width / 2,
                        (float)GraphicsDevice.Viewport.Height / 2),
                    Color.Black);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
