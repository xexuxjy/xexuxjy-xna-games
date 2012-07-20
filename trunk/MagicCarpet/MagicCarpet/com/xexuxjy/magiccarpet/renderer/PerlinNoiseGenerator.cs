using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet;

namespace com.xexuxjy.magiccarpet.renderer
{
    public static class PerlinNoiseGenerator
    {
        static PerlinNoiseGenerator()
        {
                m_perlinNoiseEffect = Globals.Game.Content.Load<Effect>("Effects/Perlin");
                m_screenQuad = new ScreenQuad(Globals.Game);
                m_screenQuad.Initialize();
                m_baseNoiseTexture = CreateStaticMap(64);
        }


        public static void GeneratePerlinNoise(int width,RenderTarget2D renderTarget)
        {
            Globals.GraphicsDevice.SetRenderTarget(renderTarget);
            Globals.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            m_perlinNoiseEffect.Parameters["xTexture"].SetValue(m_baseNoiseTexture);
            m_perlinNoiseEffect.Parameters["xOvercast"].SetValue(1.1f);
            //m_perlinNoiseEffect.Parameters["xTime"].SetValue(time / 1000.0f);
            m_perlinNoiseEffect.Parameters["xTime"].SetValue(1f);
            
            foreach (EffectPass pass in m_perlinNoiseEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_screenQuad.Draw();
            }
            Globals.GraphicsDevice.SetRenderTarget(null);
        }


        private static Texture2D CreateStaticMap(int resolution)
        {
            Random rand = new Random();
            Color[] noisyColors = new Color[resolution * resolution];
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    noisyColors[x + y * resolution] = new Color(new Vector3((float)rand.Next(1000) / 1000.0f, 0, 0));
                }
            }

            Texture2D noiseImage = new Texture2D(Globals.GraphicsDevice, resolution, resolution, false, SurfaceFormat.Color);
            noiseImage.SetData(noisyColors);
            return noiseImage;
        }

        private static Texture2D m_baseNoiseTexture;
        private static Effect m_perlinNoiseEffect;
        private static ScreenQuad m_screenQuad;
    }
}
