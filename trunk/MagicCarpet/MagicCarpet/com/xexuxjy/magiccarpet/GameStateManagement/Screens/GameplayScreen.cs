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
using com.xexuxjy.magiccarpet.manager;
using com.xexuxjy.magiccarpet.terrain;
using Dhpoware;
using com.xexuxjy.magiccarpet.collision;
using BulletXNA.LinearMath;
using System.Collections.Generic;
using com.xexuxjy.magiccarpet.util.console;
using com.xexuxjy.magiccarpet.util.debug;
using com.xexuxjy.magiccarpet.gui;
using com.xexuxjy.magiccarpet.control;
using com.xexuxjy.magiccarpet.camera;
//using com.xexuxjy.magiccarpet.control;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
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
            try
            {
                Globals.LoadConfig();

                Globals.GameObjectManager = new GameObjectManager(this);
                Globals.GameObjectManager.CreateInitialComponents();
                //Globals.GameObjectManager.AddAndInitializeObject(Globals.GameObjectManager);
                


                if (m_content == null)
                {
                    m_content = new ContentManager(ScreenManager.Game.Services, "Content");
                }

                m_gameFont = m_content.Load<SpriteFont>("fonts/gamefont");

                m_playerHud = new PlayerHud(this);
                m_playerHud.Initialise();


                m_playerController = new PlayerController(m_playerHud);
                Globals.playerController = m_playerController;


                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;            	
            }
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
            try
            {
                m_updateCalls++;
                base.Update(gameTime, otherScreenHasFocus, false);

                // Gradually fade in or out depending on whether we are covered by the pause screen.
                if (coveredByOtherScreen)
                    pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
                else
                    pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

                if (IsActive)
                {
                    Globals.GameObjectManager.Update(gameTime);
                    Globals.s_currentCameraFrustrum.Matrix = Globals.Camera.ViewMatrix * Globals.Camera.ProjectionMatrix;

                }

                if (Globals.s_initialScript != null && !loadedInitialScript)
                {
                    loadedInitialScript = true;
                    Globals.SimpleConsole.LoadScript(Globals.s_initialScript);
                }
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input,GameTime gameTime)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }



            m_playerController.HandleInput(input,gameTime);
            //m_playerHud.HandleInput(input);
            //Globals.Camera.HandleInput(input);

            //foreach (GuiComponent guiComponent in m_guiComponents)
            //{
            //    if (guiComponent.HasGuiControl)
            //    {
            //        guiComponent.HandleInput(input);
            //    }
            //}
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            try
            {
                m_drawCalls++;
                ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
                base.Draw(gameTime);

                // reset the blendstats as spritebatch probably trashed them.
                Globals.Game.GraphicsDevice.BlendState = BlendState.Opaque;
                Globals.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                Globals.GameObjectManager.Draw(gameTime);

                // If the game is transitioning on or off, fade it out to black.
                if (TransitionPosition > 0 || pauseAlpha > 0)
                {
                    float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                    ScreenManager.FadeBackBufferToBlack(alpha);
                }
            }
            catch (System.Exception ex)
            {
                int ibreak = 0;            	
            }
        }


        #endregion
        #region Fields

        protected int m_updateCalls;
        protected int m_drawCalls;


        PlayerHud m_playerHud;
        ContentManager m_content;
        SpriteFont m_gameFont;

        static bool loadedInitialScript = false;

        float pauseAlpha;

        private PlayerController m_playerController;

        private volatile GameTime gameTime;
        #endregion

    }
}













