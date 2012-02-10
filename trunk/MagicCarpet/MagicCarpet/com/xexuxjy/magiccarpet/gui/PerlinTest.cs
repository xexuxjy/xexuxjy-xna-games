using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MagicCarpet.com.xexuxjy.magiccarpet.renderer;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.gui
{
    public class PerlinTest : GuiComponent
    {
        public PerlinTest(Point topLeft, int width) : base(topLeft,width)
        {


        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }


        public override void CheckAndUpdateTexture()
        {
            if (m_textureUpdateNeeded)
            {
                m_renderTarget = new RenderTarget2D(Globals.Game.GraphicsDevice, m_width, m_width,false, SurfaceFormat.Color, DepthFormat.Depth16);
                PerlinNoiseGenerator.GeneratePerlinNoise(m_width, m_renderTarget);
                m_textureUpdateNeeded = false;
                m_texture = m_renderTarget;
        
            }
        }


        public RenderTarget2D m_renderTarget;
    }
}
