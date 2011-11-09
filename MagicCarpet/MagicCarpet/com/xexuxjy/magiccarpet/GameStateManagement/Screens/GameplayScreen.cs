#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using com.xexuxjy.magiccarpet;
using com.xexuxjy.magiccarpet.util;
using BulletXNADemos.Demos;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.utils.debug;
using com.xexuxjy.utils.console;
using com.xexuxjy.magiccarpet.manager;
using com.xexuxjy.magiccarpet.terrain;
using Dhpoware;
using com.xexuxjy.magiccarpet.collision;
using BulletXNA.LinearMath;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (m_content == null)
            {
                m_content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            m_debugDrawMode = DebugDrawModes.DBG_DrawConstraints | DebugDrawModes.DBG_DrawConstraintLimits | DebugDrawModes.DBG_DrawAabb | DebugDrawModes.DBG_DrawWireframe;

            m_gameFont = m_content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            // TODO: Add your initialization logic here
            CameraComponent camera = new CameraComponent(ScreenManager.Game);
            Globals.Camera = camera;

            Globals.DebugDraw = new XNA_ShapeDrawer(ScreenManager.Game);
            Globals.DebugDraw.SetDebugMode(m_debugDrawMode);
            if (Globals.DebugDraw != null)
            {
                Globals.DebugDraw.LoadContent();
            }

            Globals.CollisionManager = new CollisionManager(ScreenManager.Game, Globals.worldMinPos, Globals.worldMaxPos);
            m_componentCollection.Add(Globals.CollisionManager);

            Globals.Terrain = new Terrain(Vector3.Zero, ScreenManager.Game);

            Globals.GameObjectManager = new GameObjectManager(ScreenManager.Game);

            Globals.SimpleConsole = new SimpleConsole(ScreenManager.Game, Globals.DebugDraw);
            Globals.SimpleConsole.Enabled = false;

            Globals.MCContentManager = new MCContentManager(ScreenManager.Game);
            Globals.MCContentManager.Initialize();

            Globals.DebugObjectManager = new DebugObjectManager(ScreenManager.Game, Globals.DebugDraw);
            Globals.DebugObjectManager.Enabled = true;


            Globals.Player = (Magician)Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.magician, new Vector3(0, 10, 0));
            Globals.DebugObjectManager.DebugObject = Globals.Player;

            m_componentCollection.Add(camera);

            m_componentCollection.Add(new KeyboardController(ScreenManager.Game));
            m_componentCollection.Add(new MouseController(ScreenManager.Game));
            m_componentCollection.Add(new FrameRateCounter(ScreenManager.Game, Globals.DebugTextFPS, Globals.DebugDraw));
            m_componentCollection.Add(Globals.SimpleConsole);
            m_componentCollection.Add(Globals.DebugObjectManager);
            m_componentCollection.Add(Globals.GameObjectManager);
            m_componentCollection.Add(Globals.ScreenManager);


            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            m_content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                foreach (GameComponent gameComponent in m_componentCollection)
                {
                    gameComponent.Update(gameTime);
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }



        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            Globals.CollisionManager.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
        #region Fields

        ContentManager m_content;
        SpriteFont m_gameFont;

        DebugDrawModes m_debugDrawMode;

        GameComponentCollection m_componentCollection = new GameComponentCollection();

        Random random = new Random();

        float pauseAlpha;

        #endregion

    }
}
