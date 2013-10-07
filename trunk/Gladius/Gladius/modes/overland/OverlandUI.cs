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
        public override void DrawElement(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch)
        {
            Vector4 boxBackgroundV4 = new Vector4(0.5f,0.5f,0.5f,0.3f);
            Texture2D boxBorder = Globals.GlobalContentManager.GetColourTexture(Color.White);
            Texture2D boxBackground = Globals.GlobalContentManager.GetV4Texture(boxBackgroundV4);
            Rectangle topBounds = new Rectangle(500,10,300,100);
            int inset = 3;
            Rectangle topInnerBounds = Globals.InsetRectangle(topBounds, inset);

            Color coverageColor = Color.White;
            //coverageColor.A = 50;
            coverageColor *= 0.35f;


            //spriteBatch.Draw(boxBorder, topBounds, Color.White);
            spriteBatch.Draw(boxBackground, topInnerBounds, coverageColor);
            m_infoText.Clear();
            m_infoText.ConcatFormat("Days : {0}", m_overland.DayCount);
            spriteBatch.DrawString(m_spriteFont, m_infoText, new Vector2(topBounds.X + 100, topBounds.Y + 10), Color.White);


            float rotation = (MathHelper.TwoPi) * m_overland.TimeOfDayFraction;
            int offset = 32;
            Vector2 pos = new Vector2(topBounds.X + offset, topBounds.Y + offset);

            pos.X += 10;
            pos.Y += 10;


            int width = 64;
            //Rectangle destRectangle = new Rectangle((int)pos.X+offset,(int)pos.Y+offset,width,width);
            Vector2 origin = new Vector2(width / 2);

            //Vec
            //spriteBatch.Draw(m_dayNightTexture, destRectangle, null, Color.White, rotation,origin, SpriteEffects.None, 0);
            spriteBatch.Draw(m_dayNightTexture, (pos ), null, Color.White, rotation, origin, 1f, SpriteEffects.None, 1);
                //spriteBatch.Draw(m_dayNightTexture,shipPosition + origin, null, Color.White, rotation, origin, 1, SpriteEffects.None, 1);


            Rectangle bottomBounds = new Rectangle(50, 300, 600, 100);
            Rectangle bottomInnerBounds = Globals.InsetRectangle(bottomBounds, inset);

            //spriteBatch.Draw(boxBorder, bottomBounds, Color.White);

            spriteBatch.Draw(boxBackground, bottomInnerBounds, coverageColor);
            m_infoText.Clear();
            Town nearestTown = m_overland.TownManager.NearestTown(m_overland.Party.Position);
            m_infoText.ConcatFormat("Gold: {0}\nNearby Town: {1}\nRank: {2} ", m_overland.Party.Gold,nearestTown!= null?nearestTown.Name:"None",m_overland.Party.LeagueRank);
            spriteBatch.DrawString(m_spriteFont, m_infoText, new Vector2(bottomBounds.X + 10, bottomBounds.Y + 10), Color.White);


        }

        private Texture2D m_dayNightTexture;
        private StringBuilder m_infoText = new StringBuilder();
        private SpriteFont m_spriteFont;
        private OverlandScreen m_overland;
    }
}
