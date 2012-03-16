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
        }

        //----------------------------------------------------------------------------------------------

        public void HandleInput(InputState inputState,GameTime gameTime)
        {
            if(!Globals.MouseEnabled)
            {
                return;
            }

            bool leftReleased = WasReleased(ref inputState.LastMouseState, ref inputState.CurrentMouseState, 0);
            bool rightReleased = WasReleased(ref inputState.LastMouseState, ref inputState.CurrentMouseState, 2);


            float pitchChange = inputState.CurrentMouseState.Y - inputState.LastMouseState.Y;
            float yawChange = inputState.CurrentMouseState.X - inputState.LastMouseState.X;

            if (!MathUtil.FuzzyZero(pitchChange))
            {
                if (pitchChange > 0)
                {
                    m_playerController.PitchUp(pitchChange);
                }
                else
                {
                    m_playerController.PitchDown(Math.Abs(pitchChange));
                }
            }

            if (!MathUtil.FuzzyZero(yawChange))
            {
                if (yawChange > 0)
                {
                    m_playerController.YawRight(yawChange);
                }
                else
                {
                    m_playerController.YawLeft(Math.Abs(yawChange));
                }
            }

            // recenter the mouse.
            Rectangle clientBounds = Globals.Game.Window.ClientBounds;
            Mouse.SetPosition(clientBounds.Width / 2, clientBounds.Height / 2);


            float scrollWheelChange = (float)(inputState.LastMouseState.ScrollWheelValue -
                                              inputState.CurrentMouseState.ScrollWheelValue);

            if (!MathUtil.FuzzyZero(scrollWheelChange))
            {
                float scrollMultiplier = 0.02f;
                scrollWheelChange *= scrollMultiplier;
                Globals.Camera.Zoom(scrollWheelChange);
            }

            if ( leftReleased || rightReleased)
            {
                IndexedVector3 startPos = Globals.Camera.Position;
                IndexedVector3 direction = Globals.Camera.ChaseDirection;
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

            // add something to draw and test collision?
            if (true)
            {
                if (Globals.CollisionManager != null)
                {

                    int rayLength = 100;
                    int normalLength = 10;
                    IndexedVector3 startPos = Globals.Camera.Position;
                    IndexedVector3 direction = Globals.Camera.ChaseDirection;
                    IndexedVector3 endPos = startPos + (direction * rayLength);

                    IndexedVector3 collisionPoint = IndexedVector3.Zero;
                    IndexedVector3 collisionNormal = IndexedVector3.Zero;

                    if (Globals.DebugDraw != null)
                    {
                        IndexedVector3 rayColor = new IndexedVector3(1, 1, 1);
                        IndexedVector3 normalColor = new IndexedVector3(1, 0, 0);
                        Globals.DebugDraw.DrawLine(ref startPos, ref endPos, ref rayColor);

                        IndexedVector3 location = Globals.DebugTextCamera;
                        IndexedVector3 colour = new IndexedVector3(1, 1, 1);

                        //if (false && Globals.CollisionManager.CastRay(startPos, endPos, ref collisionPoint, ref collisionNormal))
                        if (Globals.CollisionManager.CastRay(startPos, endPos, ref collisionPoint, ref collisionNormal))
                        {
                            Globals.cameraHasGroundContact = true;
                            Globals.cameraGroundContactPoint = collisionPoint;
                            Globals.cameraGroundContactNormal = collisionNormal;

                            
                            IndexedVector3 normalStart = collisionPoint;
                            IndexedVector3 normalEnd = collisionPoint + (collisionNormal * normalLength);
                            Globals.DebugDraw.DrawLine(ref normalStart, ref normalEnd, ref normalColor);
                            Globals.DebugDraw.DrawText(String.Format("Camera Pos[{0} Forward[{1}] Collide[{2}] Normal[{3}].", startPos, direction,collisionPoint,collisionNormal), location, colour);

                        }
                        else
                        {
                            Globals.cameraHasGroundContact = false;
                            Globals.DebugDraw.DrawText(String.Format("Camera Pos[{0} Forward[{1}].", startPos, direction), location, colour);
                        }

                    }
                }

            }
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


        private bool m_invertY = false;
        private PlayerController m_playerController;
    }
}