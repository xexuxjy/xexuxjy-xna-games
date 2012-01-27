using Microsoft.Xna.Framework;
using GameStateManagement;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace com.xexuxjy.magiccarpet.gui
{
    public abstract class GuiComponent : DrawableGameComponent
    {
        public GuiComponent(int x, int y,int width)
            : base(Globals.Game)
        {
            m_mapTopCorner = new Vector2(x, y);
            m_width = width;
            m_rectangle = new Rectangle(x, y, width, width);
        

        }

        public virtual void HandleInput(InputState inputState)
        {
        }

        public bool HasGuiControl
        {
            get{return m_hasGuiControl;}
            set{m_hasGuiControl = value;}
        }


        public static void DrawFilledCircle(Color[] texture, int step,int xPos, int yPos, int radius, Color circleColor, Color backgroundColor, bool clearFirst)
        {
            if (clearFirst)
            {
                for (int i = 0; i < texture.Length; ++i)
                {
                    texture[i] = backgroundColor;
                }
            }
            // ensure we can draw within the bounds??
            // Work out the minimum step necessary using trigonometry + sine approximation.
            int startX = xPos - radius;
            startX = Math.Max(0,startX);
            int endX = xPos + radius;
            endX = Math.Min(endX,step);
            int startY = yPos - radius;
            startY = Math.Max(0,startY);
            int endY = yPos + radius;
            endY = Math.Min(endY,step);

            int radiusSq = radius*radius;
            for(int j=startY;j<endY;++j)
            {

                int offset = j * step;
                int yoff = j - yPos;
                yoff *= yoff;

                for(int i=startX;i<endX;++i)
                {
                    int xoff = i - xPos;
                    if ((xoff * xoff) + yoff < radiusSq)
                    {
                        texture[offset + i] = circleColor;
                    }
                }
            }
        }

        public static void DrawFilledRectangle(Color[] texture, int step, int xPos, int yPos, int width,int height, Color circleColor, Color backgroundColor, bool clearFirst)
        {
            if (clearFirst)
            {
                for (int i = 0; i < texture.Length; ++i)
                {
                    texture[i] = backgroundColor;
                }
            }
            // ensure we can draw within the bounds??
            // Work out the minimum step necessary using trigonometry + sine approximation.
            int startX = xPos - (width/2);
            startX = Math.Max(0, startX);
            int endX = xPos + (width/2);
            endX = Math.Min(endX, step);
            int startY = yPos - (height/2);
            startY = Math.Max(0, startY);
            int endY = yPos + (height/2);
            endY = Math.Min(endY, step);

            for (int j = startY; j < endY; ++j)
            {

                int offset = j * step;
                int yoff = j - yPos;
                yoff *= yoff;

                for (int i = startX; i < endX; ++i)
                {
                    int xoff = i - xPos;
                    texture[offset + i] = circleColor;

                }
            }
        }


        protected Vector2 m_mapTopCorner;
        protected Rectangle m_rectangle;
        protected SpriteBatch m_spriteBatch;
        protected int m_width;
        protected Texture2D m_texture;
        protected Color[] m_colorData;

        protected bool m_enabled;
        protected bool m_hasGuiControl;
    }
}
