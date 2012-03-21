
using System;
using BulletXNA.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.util.debug
{
    public class FrameRateCounter : GameObject
    {
        ContentManager content;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        Vector3 m_location;
        IDebugDraw m_debugDraw;

        public FrameRateCounter(Vector3 location, IDebugDraw debugDraw)
            : base(GameObjectType.gui)
        {
            m_location = location;
            m_debugDraw = debugDraw;
        }

        public void SetLocation(Vector3 location)
        {
            m_location = location;
        }


        //protected override void UnloadGraphicsContent(bool unloadAllContent)
        //{
        //    if (unloadAllContent)
        //        content.Unload();
        //}


        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);
            Vector3 colour = new Vector3(1, 1, 1);
            m_debugDraw.DrawText(fps, m_location, colour);
        }
    }
}
