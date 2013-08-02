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
using Dhpoware;
using GameStateManagement;
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

            SetupArena();
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
            m_inputstate.Update();
            m_camera.HandleInput(m_inputstate);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            m_arenaRenderer.Draw(m_arena,m_camera,GraphicsDevice);

            base.Draw(gameTime);



        }

        public void SetupArena()
        {
            m_inputstate = new InputState();

            m_camera = new Dhpoware.CameraComponent(this);
            IGameComponent igc = m_camera as IGameComponent;
            Components.Add(igc);

            float aspect = GraphicsDevice.Viewport.AspectRatio;
            m_camera.Perspective(((float)Math.PI / 4f), aspect, 0.1f, 1000f);
            m_arena = new Arena(32, 32);

            m_camera.Position = new Vector3(0, 2, 0);
            //m_camera.LookAt(Vector3.Zero);
            m_camera.CurrentBehavior = Camera.Behavior.Spectator;
            //m_camera.LookAt(Vector3.Zero, new Vector3(0,-1,1), Vector3.Up);

            m_arenaRenderer = new SimpleArenaRenderer();
            m_arenaRenderer.LoadContent(this,GraphicsDevice);
        }



        Arena m_arena;
        SimpleArenaRenderer m_arenaRenderer;
        ICamera m_camera;
        InputState m_inputstate;

    }
}
