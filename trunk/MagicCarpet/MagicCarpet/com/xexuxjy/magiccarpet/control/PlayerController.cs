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
            m_mouseController.HandleInput(inputState,gameTime);
            m_keyboardController.HandleInput(inputState,gameTime);
            m_playerHud.HandleInput(inputState);
            //Globals.Camera.HandleInput(inputState);


            if (inputState.IsNewButtonPress(Buttons.Y))
            {
                m_playerHud.ToggleSpellSelector();
            }


            if (Globals.Player != null)
            {
                Globals.Player.PlayerControlled = true;


                Matrix im = Globals.Player.WorldTransform;
                Vector3 Up = im.Up;
                Vector3 Right = im.Right;
                Vector3 Forward = im.Forward;

                // Update camera etc?
                // Correct the X axis steering when the ship is upside down
                //if (Up.Y < 0)
                //{
                //    rotationAmount.X = -rotationAmount.X;
                //}


                // Create rotation matrix from rotation amount
                Matrix rotationMatrix =
                    Matrix.CreateFromAxisAngle(Right, m_currentPitch) *
                    Matrix.CreateRotationY(m_currentYaw);

                // Rotate orientation vectors
                Vector3 Direction = Vector3.TransformNormal(Forward, rotationMatrix);
                Up = Vector3.TransformNormal(Up, rotationMatrix);

                // Re-normalize orientation vectors
                // Without this, the matrix transformations may introduce small rounding
                // errors which add up over time and could destabilize the ship.
                Direction.Normalize();
                Up.Normalize();

                // Re-calculate Right
                Right = Vector3.Cross(Direction, Up);

                // The same instability may cause the 3 orientation vectors may
                // also diverge. Either the Up or Direction vector needs to be
                // re-computed with a cross product to ensure orthagonality
                Up = Vector3.Cross(Right, Direction);

                Matrix world = Matrix.Identity;
                world.Forward = Direction;
                world.Up = Up;
                world.Right = Right;

                Globals.Player.WorldTransform = world;


                Globals.Camera.ChasePosition = Globals.Player.Position;
                Globals.Camera.ChaseDirection = Globals.Player.Forward;
                Globals.Camera.Up = Globals.Player.Up;

            }
        
        
        }




        public void StepForward(float amount)
        {

        }

        public void StepBackward(float amount)
        {

        }

        public void StepLeft(float amount)
        {

        }

        public void StepRight(float amount)
        {

        }

        public void StepUp(float amount)
        {

        }

        public void StepDown(float amount)
        {

        }


        public void YawLeft(float amount)
        {
            m_currentYaw -= amount;
                 
        }

        public void YawRight(float amount)
        {
            m_currentYaw += amount;

        }

        public void PitchUp(float amount)
        {
            m_currentPitch += amount;

        }

        public void PitchDown(float amount)
        {
            m_currentPitch -= amount;
        }


        private float m_currentYaw;
        private float m_currentPitch;

        private PlayerHud m_playerHud;
        private MouseController m_mouseController;
        private KeyboardController m_keyboardController;   

    }
}
