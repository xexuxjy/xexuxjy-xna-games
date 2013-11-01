#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Gladius.actors;
using Gladius.renderer;
using System.Text;
using Gladius.control;
using Gladius.util;
using Gladius.gamestatemanagement.screens;
using Gladius.gamestatemanagement.screenmanager;
#endregion

namespace Gladius
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public MainGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 1024;
            Content.RootDirectory = "Content";
            Globals.EventLogger = new EventLogger(this,null);
            Globals.EventLogger.Enabled = true;
            Globals.CameraManager = new CameraManager(this);
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

            // TODO: use this.Content to load your game content here

            //SetupArena();
            m_screenManager = new ScreenManager(this);
            m_screenManager.AddScreen(new MainMenuScreen(),null);
            //m_screenManager.AddScreen(new ArenaScreen(), null);
            //m_screenManager.AddScreen(new ShopScreen(), null);
            //m_screenManager.AddScreen(new OverlandScreen(), null);
            //Globals.GlobalContentManager = new ThreadSafeContentManager(this, this.Services);

            Components.Add(m_screenManager);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            //m_inputstate.Update();
            //m_camera.HandleInput(m_inputstate);
            Globals.CameraManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        //protected override void Draw(GameTime gameTime)
        //{
        //    GraphicsDevice.Clear(Color.CornflowerBlue);

        //    // TODO: Add your drawing code here

        //    m_arenaRenderer.Draw(m_camera,GraphicsDevice);
        //    //DrawCameraDebugText();
        //    base.Draw(gameTime);



        //}

        //public void SetupArena()
        //{
        //    m_inputstate = new InputState();


        //    m_spriteBatch = new SpriteBatch(GraphicsDevice);

        //    // Load the sprite font. The sprite font has a 3 pixel outer glow
        //    // baked into it so we need to decrease the spacing so that the
        //    // SpriteFont will render correctly.
        //    m_spriteFont = Content.Load<SpriteFont>("GameFont");
        //    m_spriteFont.Spacing = -4.0f;


        //    Globals.UserControl = new UserControl(this,m_inputstate);
        //    Components.Add(Globals.UserControl);

        //    m_camera = new Dhpoware.CameraComponent(this);
        //    IGameComponent igc = m_camera as IGameComponent;
        //    Components.Add(igc);
        //    float aspect = GraphicsDevice.Viewport.AspectRatio;
        //    m_camera.Position = new Vector3(0, 5, -10);
        //    Globals.Camera = m_camera;
            
        //    m_arena = new Arena(32, 32);

        //    //m_camera.Position = new Vector3(0, 2, 0);
        //    //m_camera.LookAt(Vector3.Zero);
        //    m_camera.CurrentBehavior = Camera.Behavior.FirstPerson;
        //    //m_camera.LookAt(Vector3.Zero, new Vector3(0,-1,1), Vector3.Up);

        //    BaseActor ba1 = new BaseActor();
        //    BaseActor ba2 = new BaseActor();
        //    BaseActor ba3 = new BaseActor();
        //    BaseActor ba4 = new BaseActor();

        //    ba1.CurrentPoint = new Point(10, 10);
        //    ba2.CurrentPoint = new Point(10, 20);
        //    ba3.CurrentPoint = new Point(20, 10);
        //    ba4.CurrentPoint = new Point(20, 20);

        //    m_arena.MoveActor(ba1, ba1.CurrentPoint);
        //    m_arena.MoveActor(ba2, ba2.CurrentPoint);
        //    m_arena.MoveActor(ba3, ba3.CurrentPoint);
        //    m_arena.MoveActor(ba4, ba4.CurrentPoint);

        //    m_arenaRenderer = new SimpleArenaRenderer(m_arena);
        //    m_arenaRenderer.LoadContent(this,GraphicsDevice);
        //}


        //private void DrawCameraDebugText()
        //{
        //    string text = null;
        //    StringBuilder buffer = new StringBuilder();
        //    Vector2 fontPos = new Vector2(1.0f, 1.0f);

        //        buffer.Append("Camera:\n");
        //        buffer.AppendFormat("  Behavior: {0}\n", m_camera.CurrentBehavior);
        //        buffer.AppendFormat("  Position: x:{0} y:{1} z:{2}\n",
        //            m_camera.Position.X.ToString("#0.00"),
        //            m_camera.Position.Y.ToString("#0.00"),
        //            m_camera.Position.Z.ToString("#0.00"));
        //        buffer.AppendFormat("  Velocity: x:{0} y:{1} z:{2}\n",
        //            m_camera.CurrentVelocity.X.ToString("#0.00"),
        //            m_camera.CurrentVelocity.Y.ToString("#0.00"),
        //            m_camera.CurrentVelocity.Z.ToString("#0.00"));
        //        buffer.AppendFormat("  Rotation speed: {0}\n",
        //            m_camera.RotationSpeed.ToString("#0.00"));

        //        if (m_camera.PreferTargetYAxisOrbiting)
        //            buffer.Append("  Target Y axis orbiting\n\n");
        //        else
        //            buffer.Append("  Free orbiting\n\n");

     
        //        text = buffer.ToString();

        //    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        //    spriteBatch.DrawString(m_spriteFont, text, fontPos, Color.Yellow);
        //    spriteBatch.End();
        //}


        //SpriteFont m_spriteFont;
        //SpriteBatch m_spriteBatch;
        //Arena m_arena;
        //SimpleArenaRenderer m_arenaRenderer;
        ////ICamera m_camera;
        //CameraComponent m_camera;
        //InputState m_inputstate;
        ScreenManager m_screenManager;
    }
}
