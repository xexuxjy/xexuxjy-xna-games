using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.gui
{
    public class EventWindow : GuiComponent
    {
        public EventWindow(int x, int y, int width)
            : base(x, y, width)
        {
            m_enabled = true;
            m_hasGuiControl = true;
            m_textureUpdateNeeded = true;
            m_maxTextLines = 10;
            m_textLines = new String[m_maxTextLines];
            m_windowColor = Color.DarkGray;
            m_windowColor.A = 40; ;
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            m_texture = new Texture2D(Game.GraphicsDevice, m_width, m_width, false, SurfaceFormat.Color);
            m_colorData = new Color[m_width * m_width];
            DrawFilledRectangle(m_colorData, m_width, 0, 0, m_width, m_width, m_windowColor, Color.Transparent, false);
            m_texture.SetData<Color>(m_colorData);
            m_textureUpdateNeeded = false;

        }
        
        public override void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_texture, m_rectangle, m_windowColor);
            SpriteFont font = Globals.MCContentManager.EventWindowFont;
            for (int i = 0; i < m_maxTextLines ; ++i)
            {
                int index = m_currentTextStart + i +1;
                
                index = index % m_maxTextLines;
                String outputLine = m_textLines[index];
                if (!String.IsNullOrEmpty(outputLine))
                {

                    Vector2 position = new Vector2(0, i * 10);
                    position += m_componentTopCorner;
                    // draw shadowed text
                    m_spriteBatch.DrawString(font, outputLine, position, Color.Black);
                    m_spriteBatch.DrawString(font, outputLine, new Vector2(position.X+1,position.Y+1), Color.White);
                }
            }

            m_spriteBatch.End();
        }

        public void AddEventText(String text)
        {
            // wrap the buffer;
            ++m_currentTextStart;
            m_currentTextStart = m_currentTextStart % m_maxTextLines;
            m_textLines[m_currentTextStart] = text;
        }


        protected Color m_windowColor;
        protected int m_maxTextLines;
        protected int m_currentTextStart = 0;
        protected String[] m_textLines;

    }
}
