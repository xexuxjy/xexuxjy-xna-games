using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.gamestatemanagement.screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using StringLeakTest;


namespace Gladius.modes.overland
{
    public class OverlandUI : BaseUIElement
    {
        public OverlandUI(OverlandScreen overland)
        {
            Visible = true;
            m_overland = overland;
        }

        public override void LoadContent(ContentManager manager, GraphicsDevice device)
        {
            m_spriteFont = manager.Load<SpriteFont>("UI/DebugFont8");

        }
        public override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle bounds = new Rectangle(500,10,300,50);
            int inset = 3;
            Rectangle innerBounds = new Rectangle(bounds.X + inset, bounds.Y + inset, bounds.Width - (2 * inset), bounds.Height - (2 * inset));
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.White), bounds, Color.White);
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.Black), innerBounds, Color.White);
            m_infoText.Clear();
            m_infoText.ConcatFormat("Time of day : {0}", m_overland.TimeOfDayHours);
            spriteBatch.DrawString(m_spriteFont, m_infoText, new Vector2(bounds.X + 10, bounds.Y + 10), Color.White);


        }

        private StringBuilder m_infoText = new StringBuilder();
        private SpriteFont m_spriteFont;
        private OverlandScreen m_overland;
    }
}
