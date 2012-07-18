using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.util;
using GameStateManagement;
using com.xexuxjy.magiccarpet.gui;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using BulletXNA.LinearMath;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.control
{
    public class PlayerController
    {

        public PlayerController(PlayerHud playerHud)
        {
            m_keyboardController = new KeyboardController(this);
            m_mouseController = new MouseController(this);

            m_playerHud = playerHud;

            Globals.Game.Activated += new EventHandler<EventArgs>(Game_Activated);
            Globals.Game.Deactivated += new EventHandler<EventArgs>(Game_Deactivated);
            
        }

        void Game_Activated(object sender, EventArgs e)
        {
            // reset?
            m_skippedFirst = false;        
        }

        void Game_Deactivated(object sender, EventArgs e)
        {
            int ibreak = 0;
            m_skippedFirst = false;
        }



        public virtual void HandleInput(InputState inputState,GameTime gameTime)
        {
            if (!Globals.Game.IsActive)
            {
                return;
            }
            if (!m_skippedFirst)
            {
                // synch the two so we don't get odd updates??
                inputState.LastMouseState = inputState.CurrentMouseState;
                m_skippedFirst = true;
                return;
            }
 
            // reset values?
            m_currentTranslation = Vector3.Zero;

            m_mouseController.HandleInput(inputState,gameTime);
            m_keyboardController.HandleInput(inputState,gameTime);
            m_playerHud.HandleInput(inputState);
            //Globals.Camera.HandleInput(inputState);

            // reset to test
            //m_currentYaw = 0;
            //m_currentPitch = 0;


            if (inputState.IsNewButtonPress(Buttons.Y))
            {
                m_playerHud.ToggleSpellSelector();
            }


            if (Globals.Player != null)
            {
                Globals.Player.PlayerControlled = true;
                Globals.Camera.FollowTarget = Globals.Player;


                // adjust position.
                // need to convert the absolute movements into player relative ones.

                Matrix im = Globals.Player.WorldTransform;
                Vector3 position = im.Translation;

                Quaternion q1 = Quaternion.CreateFromAxisAngle(Vector3.Up, m_currentYaw);
                Quaternion q2 = Quaternion.CreateFromAxisAngle(Vector3.Right, m_currentPitch);
                Quaternion qresult = q1 * q2;
                Matrix qmatrix = Matrix.CreateFromQuaternion(qresult);
                qmatrix.Translation = position;

                // make movement relative
                Vector3 relativeMovement = Vector3.TransformNormal(m_currentTranslation, qmatrix);


                qmatrix.Translation += relativeMovement;
                Globals.Player.WorldTransform = qmatrix;

            }
        }




        public void StepForward(float amount)
        {
            m_currentTranslation += Vector3.Forward * amount;
        }

        public void StepBackward(float amount)
        {
            m_currentTranslation += Vector3.Backward * amount;

        }

        public void StepLeft(float amount)
        {
            m_currentTranslation.X -= amount;

        }

        public void StepRight(float amount)
        {
            m_currentTranslation.X += amount;

        }

        public void StepUp(float amount)
        {
            m_currentTranslation.Y += amount;

        }

        public void StepDown(float amount)
        {
            m_currentTranslation.Y -= amount;
        }

        public void UpdateYaw(float amount)
        {
            if (amount < 0)
            {
                int ibreak = 0;
            }
            m_lastYaw = m_currentYaw;
            m_currentYaw += amount;
                
        }

        public void UpdatePitch(float amount)
        {
            m_lastPitch = m_currentPitch;
            m_currentPitch += amount;

        }

        public float CurrentYaw
        {
            get { return m_currentYaw; }
        }
        public float CurrentPitch
        {
            get { return m_currentPitch; }
        }

        public float LastYaw
        {
            get { return m_lastYaw; }
        }
        public float LastPitch
        {
            get { return m_lastPitch; }
        }




        private float m_currentYaw;
        private float m_currentPitch;
        private float m_lastYaw;
        private float m_lastPitch;
        private Vector3 m_currentTranslation;


        private PlayerHud m_playerHud;
        private MouseController m_mouseController;
        private KeyboardController m_keyboardController;

        private bool m_skippedFirst = false;
 
    }
}
