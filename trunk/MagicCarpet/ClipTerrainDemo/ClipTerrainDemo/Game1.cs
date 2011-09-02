using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Dhpoware;
using BulletXNA.LinearMath;
using BulletXNADemos.Demos;

namespace ClipTerrainDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public Game1()
        {
            m_graphics = new GraphicsDeviceManager(this);

            m_debugDraw = new XNA_ShapeDrawer(this);
            Content.RootDirectory = "Content";
            m_debugDrawMode = DebugDrawModes.DBG_DrawConstraints | DebugDrawModes.DBG_DrawConstraintLimits | DebugDrawModes.DBG_DrawAabb | DebugDrawModes.DBG_DrawWireframe;
            m_debugDraw.SetDebugMode(m_debugDrawMode);
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
            m_debugDraw.LoadContent();

            m_camera = new CameraComponent(this);
            
            
            //m_clipTerrain = new ClipTerrain(this, m_camera);
            //m_clipTerrain.Initialize();
            //Components.Add(m_clipTerrain);

            m_clipLevels.Add(new ClipLevel(this, m_camera, Vector3.Zero, 0));
            m_clipLevels.Add(new ClipLevel(this, m_camera, Vector3.Zero, 1));
            //m_clipLevels.Add(new ClipLevel(this, m_camera, Vector3.Zero, 2));
            //m_clipLevels.Add(new ClipLevel(this, m_camera, Vector3.Zero, 3));
            //m_clipLevels.Add(new ClipLevel(this, m_camera, Vector3.Zero, 4));
            //m_clipLevels.Add(new ClipLevel(this, m_camera, Vector3.Zero, 5));

            foreach (ClipLevel clipLevel in m_clipLevels)
            {
                clipLevel.Initialize();
                Components.Add(clipLevel);
            }
            
            
            Components.Add(m_camera);
            Components.Add(new FrameRateCounter(this, new Vector3(20,0,20),m_debugDraw));


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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }


            // TODO: Add your update logic here

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
            Matrix view = m_camera.ViewMatrix;
            Matrix projection = m_camera.ProjectionMatrix;
            m_debugDraw.RenderDebugLines(gameTime, ref view, ref projection);
            m_debugDraw.RenderOthers(gameTime, ref view, ref projection);


            base.Draw(gameTime);
        }

        
        XNA_ShapeDrawer m_debugDraw;
        DebugDrawModes m_debugDrawMode;
        CameraComponent m_camera;
        ClipTerrain m_clipTerrain;
        List<ClipLevel> m_clipLevels = new List<ClipLevel>();
        GraphicsDeviceManager m_graphics;

    }

}
