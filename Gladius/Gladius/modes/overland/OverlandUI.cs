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
            m_spriteFont = manager.Load<SpriteFont>("UI/OverlandGame");
            m_dayNightTexture = manager.Load<Texture2D>("UI/DayAndNight");
        }
        public override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle bounds = new Rectangle(500,10,300,100);
            int inset = 3;
            Rectangle innerBounds = new Rectangle(bounds.X + inset, bounds.Y + inset, bounds.Width - (2 * inset), bounds.Height - (2 * inset));
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.White), bounds, Color.White);
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.Black), innerBounds, Color.White);
            m_infoText.Clear();
            m_infoText.ConcatFormat("Days : {0}", m_overland.DayCount);
            spriteBatch.DrawString(m_spriteFont, m_infoText, new Vector2(bounds.X + 100, bounds.Y + 10), Color.White);


            float rotation = (MathHelper.TwoPi) * m_overland.TimeOfDayFraction;
            int offset = 32;
            Vector2 pos = new Vector2(bounds.X + offset, bounds.Y + offset);

            pos.X += 10;
            pos.Y += 10;


            int width = 64;
            //Rectangle destRectangle = new Rectangle((int)pos.X+offset,(int)pos.Y+offset,width,width);
            Vector2 origin = new Vector2(width / 2);

            //Vec
            //spriteBatch.Draw(m_dayNightTexture, destRectangle, null, Color.White, rotation,origin, SpriteEffects.None, 0);
            spriteBatch.Draw(m_dayNightTexture, (pos ), null, Color.White, rotation, origin, 1f, SpriteEffects.None, 1);
                //spriteBatch.Draw(m_dayNightTexture,shipPosition + origin, null, Color.White, rotation, origin, 1, SpriteEffects.None, 1);
        }

        private Texture2D m_dayNightTexture;
        private StringBuilder m_infoText = new StringBuilder();
        private SpriteFont m_spriteFont;
        private OverlandScreen m_overland;
    }
}
