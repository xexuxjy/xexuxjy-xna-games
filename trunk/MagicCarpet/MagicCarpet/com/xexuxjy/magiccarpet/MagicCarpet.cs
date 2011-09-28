using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using com.xexuxjy.magiccarpet.terrain;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.collision;
using BulletXNADemos.Demos;
using Dhpoware;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.utils.console;
using com.xexuxjy.magiccarpet.manager;
using com.xexuxjy.utils.debug;

namespace com.xexuxjy.magiccarpet
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MagicCarpet : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DebugDrawModes m_debugDrawMode;
        public MagicCarpet()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            m_debugDrawMode = DebugDrawModes.DBG_DrawConstraints | DebugDrawModes.DBG_DrawConstraintLimits | DebugDrawModes.DBG_DrawAabb | DebugDrawModes.DBG_DrawWireframe;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            CameraComponent camera = new CameraComponent(this);
            Globals.Camera = camera;

            Globals.DebugDraw = new XNA_ShapeDrawer(this);
            Globals.DebugDraw.SetDebugMode(m_debugDrawMode);
            if (Globals.DebugDraw != null)
            {
                Globals.DebugDraw.LoadContent();
            }

            Globals.CollisionManager = new CollisionManager(this,Globals.worldMinPos,Globals.worldMaxPos);
            Components.Add(Globals.CollisionManager);

            Globals.Terrain = new Terrain(Vector3.Zero, this);

            Globals.GameObjectManager = new GameObjectManager(this);

            Globals.SimpleConsole = new SimpleConsole(this,Globals.DebugDraw);
            Globals.SimpleConsole.Enabled = false;

            Globals.MCContentManager = new MCContentManager(this);
            Globals.MCContentManager.Initialize();

            Globals.DebugObjectManager = new DebugObjectManager(this,Globals.DebugDraw);
            Globals.DebugObjectManager.Enabled = true;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            Components.Add(camera);
            //Components.Add(Globals.Terrain);
            Components.Add(new KeyboardController(this));
            Components.Add(new MouseController(this));
            Components.Add(new FrameRateCounter(this,Globals.DebugTextFPS,Globals.DebugDraw));
            Components.Add(Globals.SimpleConsole);
            Components.Add(Globals.DebugObjectManager);
            Components.Add(Globals.GameObjectManager);
            
            base.Initialize();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            // do these last.
            if (Globals.DebugDraw != null)
            {
                Matrix view = Globals.Camera.ViewMatrix;
                Matrix projection = Globals.Camera.ProjectionMatrix;
                ((XNA_ShapeDrawer)Globals.DebugDraw).RenderDebugLines(gameTime, ref view, ref projection);
                ((XNA_ShapeDrawer)Globals.DebugDraw).RenderOthers(gameTime, ref view, ref projection);
            }
            // TODO: Add your drawing code here

        }
    }
}
