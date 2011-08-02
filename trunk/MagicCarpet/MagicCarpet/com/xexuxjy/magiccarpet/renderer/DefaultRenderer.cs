using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace com.xexuxjy.magiccarpet.renderer
{
    public class DefaultRenderer : DrawableGameComponent
    {
        public DefaultRenderer(MagicCarpet game)
            : base(game)
        {
            game.Components.Add(this);
        }


        public virtual void DrawDebugAxes(GraphicsDevice graphicsDevice)
        {

        }

        public virtual void DrawBoundingBox(GraphicsDevice graphicsDevice)
        {

        }

        public virtual bool ShouldDrawBoundingBox()
        {
            return m_drawBoundingBox;
        }

        public virtual void DrawBasicEffect(GraphicsDevice graphicsDevice, ref Matrix view,ref Matrix world,ref Matrix projection)
        {

        }


        protected Texture2D m_texture;
        protected bool m_drawBoundingBox;
        protected BasicEffect m_basicEffect;    
    }
}
