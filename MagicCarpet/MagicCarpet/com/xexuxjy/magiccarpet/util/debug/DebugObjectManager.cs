using System;
using System.Collections.Generic;
using System.Text;
using com.xexuxjy.utils.debug;
using com.xexuxjy.magiccarpet.debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace com.xexuxjy.utils.debug
{
    public class DebugObjectManager : DebugWindow
    {

        public DebugObjectManager(Game game) : base("DebugObjectManager",game)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            String outputString = "Not Active";
            if (m_debuggable != null && m_debuggable.DebugEnabled)
            {
                outputString = m_debuggable.DebugText;
            }

            //SpriteFont font = Globals.debugFont;
            //SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState, Matrix.Identity);
            //SpriteBatch.DrawString(font, outputString, new Vector2(0f, 0f), Microsoft.Xna.Framework.Graphics.Color.Yellow);
            //SpriteBatch.End();

        }

        public IDebuggable DebugObject
        {
            get { return m_debuggable; }
            set { m_debuggable = value; }
        }

        private IDebuggable m_debuggable;

    }
}
