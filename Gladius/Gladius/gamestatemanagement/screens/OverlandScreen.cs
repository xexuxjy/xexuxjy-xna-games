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
using Gladius.modes.overland;

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
            m_gameFont = m_content.Load<SpriteFont>("UI/DebugFont8");


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

            //Matrix model = Matrix.CreateFromQuaternion(m_party.Rotation);
            //Globals.Camera.Target = m_party.LookAtPoint;
            //Globals.Camera.Up = model.Up;

            //Vector3 direction = (3*model.Forward) + model.Up;
            //direction.Normalize();

            //Globals.Camera.TargetDirection = direction;
            //float followDistance = 5f;
            //Vector3 offset = model.Backward* followDistance;
            //camera.Rotate((forwardSpeed >= 0.0f) ? heading : -heading, 0.0f);
            //Globals.Camera.CurrentBehavior = Dhpoware.Camera.Behavior.ThirdPerson;
            //Globals.Camera.Orientation = m_party.Rotation;
            //Globals.Camera.LookAt(m_party.LookAtPoint);
            //camera.Update(gameTime);


            //Globals.Camera.LookAt(m_party.LookAtPoint + offset, m_party.LookAtPoint, Vector3.Up);
            //Globals.Camera.

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
            //ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            ScreenManager.GraphicsDevice.Clear(m_terrain.AmbientLightColor);
            base.Draw(gameTime);

            ScreenManager.Game.GraphicsDevice.BlendState = BlendState.Opaque;
            ScreenManager.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            m_screenComponents.Draw(gameTime);


            // bit yucky... ui elements have one form of draw or another. not both
            foreach (IUIElement uiElement in m_uiElementsList)
            {
                if (uiElement.Visible)
                {
                    uiElement.DrawElement(gameTime, ScreenManager.Game.GraphicsDevice, Globals.Camera);
                }
            }

            Globals.DrawCameraDebugText(m_spriteBatch, m_gameFont, ScreenManager.FPS);


            ScreenManager.Game.GraphicsDevice.BlendState = BlendState.Opaque;
            ScreenManager.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        public void SetupOverland()
        {
            ChaseCamera chaseCamera = new ChaseCamera();
            Globals.Camera = chaseCamera;

            Globals.Camera.UpdateInput(ScreenManager.input);
            //Globals.Camera.CurrentBehavior = Dhpoware.Camera.Behavior.ThirdPerson;
            //Globals.Camera.EnableSpringSystem = true;
            m_screenComponents.Components.Add(Globals.Camera);
            //chaseCamera.DesiredPositionOffset = new Vector3(0,0,-3f);

            Globals.Camera.Position = new Vector3(0, 160, 0);
            //Globals.Camera.Acceleration = new Vector3(5);
            //Globals.Camera.Velocity = new Vector3(25);

            m_terrain = new Terrain(this);
            m_terrain.LoadContent(m_content);

            m_townManager = new TownManager(this,m_terrain);
            m_townManager.LoadContent();

            m_party = new Party(this,m_terrain);
            m_party.LoadContent();
            m_party.RegisterListeners();
            m_screenComponents.Components.Add(m_party);


            chaseCamera.LookAtOffset = Vector3.Zero;//new Vector3(0.0f, 2, -2.2f) *  m_party.ModelHeight;
            chaseCamera.DesiredPositionOffset = new Vector3(0, 0.5f, 4.0f) * m_party.ModelHeight; ;


            m_screenComponents.Components.Add(m_townManager);

            m_screenComponents.Components.Add(m_terrain);
        }

        public Party Party
        {
            get { return m_party; }
        }

        TownManager m_townManager;
        Party m_party;
        Terrain m_terrain;
    }
}
