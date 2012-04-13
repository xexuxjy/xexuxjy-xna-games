using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.control;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.util
{
    public class MouseController 
    {
        public MouseController(PlayerController playerController)
        {
            m_playerController = playerController;
            CenterMouse();
        }

        //----------------------------------------------------------------------------------------------

        public void CenterMouse()
        {

            // recenter the mouse.
            Rectangle clientBounds = Globals.Game.Window.ClientBounds;
            Mouse.SetPosition(clientBounds.Width / 2, clientBounds.Height / 2);

        }

        //----------------------------------------------------------------------------------------------

        public void HandleInput(InputState inputState, GameTime gameTime)
        {
            if(!Globals.MouseEnabled)
            {
                return;
            }

            bool leftReleased = WasReleased(ref inputState.LastMouseState, ref inputState.CurrentMouseState, 0);
            bool rightReleased = WasReleased(ref inputState.LastMouseState, ref inputState.CurrentMouseState, 2);


            float pitchChange = inputState.CurrentMouseState.Y - inputState.LastMouseState.Y;
            float yawChange = inputState.CurrentMouseState.X - inputState.LastMouseState.X;

            yawChange *= m_mouseSensitivity;
            pitchChange *= m_mouseSensitivity;


            if (!MathUtil.FuzzyZero(pitchChange))
            {
                if (m_invertY)
                {
                    pitchChange *= -1.0f;
                }
                m_playerController.UpdatePitch(pitchChange);
            }

            if (!MathUtil.FuzzyZero(yawChange))
            {
                m_playerController.UpdateYaw(yawChange);
            }

            float scrollWheelChange = (float)(inputState.LastMouseState.ScrollWheelValue -
                                              inputState.CurrentMouseState.ScrollWheelValue);

            if (!MathUtil.FuzzyZero(scrollWheelChange))
            {
                float scrollMultiplier = 0.02f;
                scrollWheelChange *= scrollMultiplier;
                //Globals.Camera.Zoom(scrollWheelChange);
            }

            if ( leftReleased || rightReleased)
            {
                Vector3 startPos = Globals.Camera.Position;
                Vector3 direction = Globals.Camera.ViewDirection;
                if(leftReleased)
                {
                    if (Globals.Player != null)
                    {
                        Globals.Player.CastSpell1(startPos, direction);
                    }
                }
                if(rightReleased)
                {
                    if (Globals.Player != null)
                    {
                        Globals.Player.CastSpell2(startPos, direction);
                    }
                }
            }

            CenterMouse();
        }


        //----------------------------------------------------------------------------------------------

        private bool WasReleased(ref MouseState old, ref MouseState current, int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                return old.LeftButton == ButtonState.Pressed && current.LeftButton == ButtonState.Released;
            }
            if (buttonIndex == 1)
            {
                return old.MiddleButton == ButtonState.Pressed && current.MiddleButton == ButtonState.Released;
            }
            if (buttonIndex == 2)
            {
                return old.RightButton == ButtonState.Pressed && current.RightButton == ButtonState.Released;
            }
            return false;
        }

        //----------------------------------------------------------------------------------------------

        //public void GenerateMouseEvents(ref MouseState oldState, ref MouseState newState)
        //{
        //    MouseFunc(ref oldState, ref newState);
        //    MouseMotionFunc(ref newState);
        //}
        private float m_mouseSensitivity = 0.001f;
        private bool m_invertY = false;
        private PlayerController m_playerController;
    }
}