﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Gladius.util;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gladius.renderer;

namespace Gladius.gamestatemanagement.screens
{
    public class OverlandScreen : GameScreen
    {

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (m_content == null)
            {
                m_content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            m_screenComponents = new ScreenGameComponents(ScreenManager.Game);

            m_spriteBatch = new SpriteBatch(ScreenManager.Game.GraphicsDevice);
            m_spriteFont = m_content.Load<SpriteFont>("UI/DebugFont8");


            SetupOverland();

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            m_updateCalls++;
            base.Update(gameTime, otherScreenHasFocus, false);

            m_screenComponents.Update(gameTime);

            foreach (IUIElement uiElement in m_uiElementsList)
            {
                uiElement.Update(gameTime);
            }




            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            m_drawCalls++;
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            m_screenComponents.Draw(gameTime);


            // bit yucky... ui elements have one form of draw or another. not both
            foreach (IUIElement uiElement in m_uiElementsList)
            {
                if (uiElement.Visible)
                {
                    uiElement.DrawElement(gameTime, ScreenManager.Game.GraphicsDevice, Globals.Camera);
                }
            }


            //m_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            //foreach (IUIElement uiElement in m_uiElementsList)
            //{
            //    if (uiElement.Visible)
            //    {
            //        uiElement.DrawElement(gameTime, m_spriteBatch);
            //    }
            //}

            //m_spriteBatch.End();

            Globals.DrawCameraDebugText(m_spriteBatch, m_spriteFont,ScreenManager.FPS);


            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        public void SetupOverland()
        {
            Globals.Camera = new Dhpoware.CameraComponent(ScreenManager.Game);
            Globals.Camera.HandleInput(ScreenManager.input);

            IGameComponent igc = Globals.Camera as IGameComponent;
            m_screenComponents.Components.Add(igc);

            Globals.Camera.Position = new Vector3(0, 5, -10);
            Globals.Camera.Acceleration = new Vector3(10);
            Globals.Camera.Velocity = new Vector3(50);

            Terrain terrain = new Terrain(ScreenManager.Game);
            terrain.LoadContent(m_content);

            m_screenComponents.Components.Add(terrain);
        }


        protected int m_updateCalls;
        protected int m_drawCalls;


        ContentManager m_content;
        SpriteFont m_spriteFont;
        SpriteBatch m_spriteBatch;

        float pauseAlpha;

        private volatile GameTime gameTime;
        protected ScreenGameComponents m_screenComponents;
        protected List<IUIElement> m_uiElementsList = new List<IUIElement>();

    }
}
