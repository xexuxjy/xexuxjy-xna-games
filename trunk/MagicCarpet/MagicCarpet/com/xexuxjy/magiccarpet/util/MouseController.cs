using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace com.xexuxjy.magiccarpet.util
{
    public class MouseController : GameComponent
    {
        public MouseController(Game game)
            : base(game)
        {

        }

        //----------------------------------------------------------------------------------------------

        public override void Update(GameTime gameTime)
        {
            //MouseState mouseState = Mouse.GetState();
            ////GenerateMouseEvents(ref m_lastMouseState, ref mouseState);

            //float mouseSpeedScape = 0.1f;
            //float scale = Globals.STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds * mouseSpeedScape;

            //float xdiff = (mouseState.X - m_lastMouseState.X) * scale;
            //if (!MathHelperExtension.CompareFloat(xdiff, 0f))
            //{
            //    Globals.Camera.Yaw += xdiff;
            //}


            //float ydiff = (mouseState.Y - m_lastMouseState.Y) * scale * (m_invertY?1:-1);
            //if (!MathHelperExtension.CompareFloat(ydiff, 0f))
            //{
            //    Globals.Camera.Pitch += ydiff;
            //}
            //// set mouse back to center
            //Rectangle r = Game.Window.ClientBounds;
            //Mouse.SetPosition((r.Width / 2), (r.Height / 2));
            MouseState mouseState = Mouse.GetState();


            if (WasReleased(ref m_lastMouseState, ref mouseState, 0))
            {

                int rayLength = 100;
                int normalLength = 10;
                Vector3 startPos = Globals.Camera.Position;
                Vector3 direction = Globals.Camera.ViewDirection;
                Vector3 endPos = startPos + (direction * rayLength);

                Vector3 collisionPoint = Vector3.Zero;
                Vector3 collisionNormal = Vector3.Zero;

                if (Globals.CollisionManager.CastRay(startPos, endPos, ref collisionPoint, ref collisionNormal))
                {
                    Vector3 normalStart = collisionPoint;
                    Globals.Terrain.AddPeak(normalStart, 10);
                }

            }

            // add something to draw and test collision?
            if (true)
            {
                if (Globals.CollisionManager != null)
                {

                    int rayLength = 100;
                    int normalLength = 10;
                    Vector3 startPos = Globals.Camera.Position;
                    Vector3 direction = Globals.Camera.ViewDirection;
                    Vector3 endPos = startPos + (direction * rayLength);

                    Vector3 collisionPoint = Vector3.Zero;
                    Vector3 collisionNormal = Vector3.Zero;

                    if (Globals.DebugDraw != null)
                    {
                        Vector3 rayColor = new Vector3(1, 1, 1);
                        Vector3 normalColor = new Vector3(1, 0, 0);
                        Globals.DebugDraw.DrawLine(ref startPos, ref endPos, ref rayColor);
                        if (Globals.CollisionManager.CastRay(startPos, endPos, ref collisionPoint, ref collisionNormal))
                        {
                            Vector3 normalStart = collisionPoint;
                            Vector3 normalEnd = collisionPoint + (collisionNormal * normalLength);
                            Globals.DebugDraw.DrawLine(ref normalStart, ref normalEnd, ref normalColor);
                        }

                        Vector3 location = new Vector3(10, 10, 0);
                        Vector3 colour = new Vector3(1, 1, 1);
                        Globals.DebugDraw.DrawText(String.Format("Camera Pos[{0} Forward[{1}].", startPos, direction), ref location, ref colour);

                    }
                }

            }



            m_lastMouseState = mouseState;

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


        MouseState m_lastMouseState;
        bool m_invertY = false;
    }
}