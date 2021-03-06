﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gladius.util
{
    // based on ThreadSafeContentManager from http://konaju.com/?p=27
    public class ThreadSafeContentManager : ContentManager
    {
        object loadLock;

        public ThreadSafeContentManager(Game game,IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            m_game = game;
            loadLock = game.GraphicsDevice;
        }

        public ThreadSafeContentManager(Game game, IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
            m_game = game;
            loadLock = game.GraphicsDevice;
        }

        public override T Load<T>(string assetName)
        {
            lock (loadLock)
            {
                return base.Load<T>(assetName);
            }
        }

        public  Texture2D GetColourTexture(Color color)
        {
            if (!m_colorMap.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(m_game.GraphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                newTexture.GetData<Color>(colorData);
                colorData[0] = new Color(color.ToVector3());
                //colorData[0].A = 128;
                newTexture.SetData(colorData);
                m_colorMap[color] = newTexture;
            }
            return m_colorMap[color];
        }

        public Texture2D GetV4Texture(Vector4 color)
        {
            if (!m_v4Map.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(m_game.GraphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                newTexture.GetData<Color>(colorData);
                colorData[0] = new Color(color);
                //colorData[0].A = 128;
                newTexture.SetData(colorData);
                m_v4Map[color] = newTexture;
            }
            return m_v4Map[color];
        }


        private Game m_game;
        private Dictionary<Color, Texture2D> m_colorMap = new Dictionary<Color, Texture2D>();
        private Dictionary<Vector4, Texture2D> m_v4Map = new Dictionary<Vector4, Texture2D>();

    }
}
