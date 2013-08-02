using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gladius.util
{
    public static class ColouredTextureDictionary
    {
        public static Texture2D GetTexture(Color color,GraphicsDevice graphicsDevice)
        {
            if (!m_colorMap.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(graphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                newTexture.GetData<Color>(colorData);
                colorData[0] = new Color(color.ToVector3());

                newTexture.SetData(colorData);
                m_colorMap[color] = newTexture;
            }
            return m_colorMap[color];
        }

        private static Dictionary<Color, Texture2D> m_colorMap = new Dictionary<Color, Texture2D>();

    }
}
