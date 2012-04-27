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

        }



        public virtual void HandleInput(InputState inputState,GameTime gameTime)
        {
            if (!Globals.Game.IsActive)
            {
                // don't do anything if we're not active.
                return;
            }
            if (!m_skippedFirst)
            {
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
                //Vector3 up = im.Up;
                Vector3 up = Vector3.Up;
                Vector3 right = im.Right;
                Vector3 forward = im.Forward;
                Vector3 position = im.Translation;

                Vector3 relativeMovement = Vector3.TransformNormal(m_currentTranslation, im);


                // Update camera etc?
                // Correct the X axis steering when the ship is upside down
                //if (Up.Y < 0)
                //{
                //    rotationAmount.X = -rotationAmount.X;
                //}


                // Create rotation matrix from rotation amount
                //Matrix rotationMatrix = Matrix.CreateFromAxisAngle(right, m_currentPitch) *
                //    Matrix.CreateRotationY(m_currentYaw);
                Matrix yawMatrix = Matrix.CreateFromAxisAngle(up,m_currentYaw);
                Matrix pitchMatrix = Matrix.CreateFromAxisAngle(right, m_currentPitch);

                Matrix rotationMatrix = yawMatrix * pitchMatrix;

                //// Rotate orientation vectors
                Vector3 direction = Vector3.TransformNormal(forward,rotationMatrix);
                up = Vector3.TransformNormal(up ,rotationMatrix);

                //// Re-normalize orientation vectors
                //// Without this, the matrix transformations may introduce small rounding
                //// errors which add up over time and could destabilize the ship.
                direction.Normalize();
                up.Normalize();

                //// Re-calculate Right
                right = Vector3.Cross(direction, up);

                // The same instability may cause the 3 orientation vectors may
                // also diverge. Either the Up or Direction vector needs to be
                // re-computed with a cross product to ensure orthagonality
                up = Vector3.Cross(right, direction);

                Matrix world = Matrix.Identity;
                world.Right = right;
                world.Up = up;
                world.Forward = direction;
                world.Translation = position;

                 //bit of extra work here.
                Globals.Player.WorldTransform = world;
                Globals.Player.Position += relativeMovement;

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
            m_currentYaw += amount;
                
        }

        public void UpdatePitch(float amount)
        {
            m_currentPitch += amount;

        }


        private float m_currentYaw;
        private float m_currentPitch;
        private Vector3 m_currentTranslation;


        private PlayerHud m_playerHud;
        private MouseController m_mouseController;
        private KeyboardController m_keyboardController;

        private bool m_skippedFirst = false;
 
    }
}
