using System;
using System.CodeDom;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Kwartet.Desktop.Cards;
using Kwartet.Desktop.Core;
using Kwartet.Desktop.Online;
using Kwartet.Desktop.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game = Microsoft.Xna.Framework.Game;

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
        private Desktop.Online.Game _game;
        private SceneManager SceneManager { get; set; }
        
        public static SpriteFont font { get; private set; }
        public RenderTarget2D renderTarget { get; private set; }

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
            renderTarget = new RenderTarget2D(GraphicsDevice, 1280, 720);
            Screen.InitScreen(renderTarget);
            this.graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
            _game = new Desktop.Online.Game(server, Content, GraphicsDevice);            
            _game.Start();
            
            server.Subscribe(ClientToServerStatus.Join, (a) =>
            {
                if (_game.PlayersConnected.Count >= 4)
                {
                    DropConnection(a);
                    return;
                }

                Console.WriteLine(a.ID);
                ConnectionInfo info = server.AddUser(a.ID, a.ServerStatus);
                int playerNum = _game.PlayerJoin(new Player(info, a.Data["name"].ToString()));
                var joinInfo = new ServerStatusHandler.JoinInfo(playerNum, a.ID);
                
                var joinMessage = new 
                    ServerMessage<ServerStatusHandler.JoinInfo>
                    (ServerToClientStatuses.JoinInfo, joinInfo);

                a.ServerStatus.Send(joinMessage);
            });
            
            base.Initialize();
        }

        private void DropConnection(ServerStatusHandler.ClientMessage clientMessage)
        {
            clientMessage.ServerStatus.Send(
                new ServerMessage
                    <ServerStatusHandler.EmptyInfo>(ServerToClientStatuses.DropConnection, new ServerStatusHandler.EmptyInfo()));
            clientMessage.ServerStatus.DropConnection();
        }

        private Texture2D c;
        private Vector2 card = new Vector2(0);

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("spritefont");
            c = Content.Load<Texture2D>("card");
            // TODO: use this.Content to load your game content here
            
            SceneManager = new SceneManager(Content, GraphicsDevice, spriteBatch, _game, server);
            SceneManager.SwitchScene(typeof(MainMenu));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private bool can = true;
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.F) && can)
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
                
                can = false;
                Task.Factory.StartNew(() => { Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                    can = true;
                });
            }
            
            
            Scene scene = SceneManager.CurrentScene;

            scene?.Update(gameTime);

            base.Update(gameTime);
        }

        
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.YellowGreen);
            
            Scene scene = SceneManager.CurrentScene;
            spriteBatch.Begin();
            string names = "";
            
            _game.PlayersConnected.ForEach(x=>names += x.Name + "\n");

            scene?.Draw(gameTime);
            
            
            spriteBatch.End();
            
            GraphicsDevice.SetRenderTarget(null);
            
            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget, GraphicsDevice.Viewport.TitleSafeArea, Color.White);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
