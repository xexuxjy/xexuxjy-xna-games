using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

    public class ColourTextureHelper
    {
        object loadLock;

        public ColourTextureHelper()
        {
        }

        public Texture2D GetColourTexture(Color color)
        {

            if (!m_colorMap.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(1,1);
                
                newTexture.SetPixel(0, 0, color);
                newTexture.Apply();
                m_colorMap[color] = newTexture;
            }
            return m_colorMap[color];
        }

        public Texture GetV4Texture(Vector4 color)
        {
            if (!m_v4Map.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(1, 1);
                newTexture.SetPixel(0,0,new Color(color.x,color.y,color.z,color.w));
                m_v4Map[color] = newTexture;
            }
            return m_v4Map[color];
        }


        private Dictionary<Color, Texture2D> m_colorMap = new Dictionary<Color, Texture2D>();
        private Dictionary<Vector4, Texture2D> m_v4Map = new Dictionary<Vector4, Texture2D>();

    }
