
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace com.xexuxjy.magiccarpet.util
{
    public class FrameRateCounter : DrawableGameComponent
    {
        ContentManager content;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        public FrameRateCounter(Game game)
            : base(game)
        {
            content = new ContentManager(game.Services);
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
            Vector3 location = Globals.DebugTextFPS;
            Vector3 colour = new Vector3(1, 1, 1);
        
            Globals.DebugDraw.DrawText(fps, ref location, ref colour);
        }
    }
}
