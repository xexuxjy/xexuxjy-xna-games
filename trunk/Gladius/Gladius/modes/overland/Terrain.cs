using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Gladius.Terrain7;
using System.IO;
using Gladius.util;
using GameStateManagement;
using Gladius.renderer;

namespace Gladius.modes.overland
{
    /*
    * Created on 11-Jan-2006
    *
    * To change the template for this generated file go to
    * Window - Preferences - Java - Code Generation - Code and Comments
    */
    public class Terrain : GameScreenComponent
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Terrain(GameScreen gameScreen)
            : base(gameScreen)
        {
        
        }

        public void LoadContent(ContentManager contentManager)
        {
            //m_heightMap = contentManager.Load<Texture2D>("Models/Terrain/WorldHeight");
            //m_surface = contentManager.Load<Texture2D>("Models/Terrain/WorldSurface");

            m_screenQuad = new ScreenQuad(Game);

            m_heightMap = contentManager.Load<Texture2D>("Models/Terrain/ESHeightMap1");
            m_surface = Globals.GlobalContentManager.GetColourTexture(Color.DarkGreen);
            m_terrainEffect = contentManager.Load<Effect>("Effects/Terrain/Terrain");
            m_terrainEffect.Parameters["BaseTexture"].SetValue(m_surface);
            BuildNormalMap(contentManager);
            //m_terrainEffect.Parameters["BaseTexture"].SetValue(m_normalMap);
            //m_position = new Vector3(m_heightMap.Width,0,m_heightMap.Height)/2f;
            Vector3 position = new Vector3(m_heightMap.Width, 0, m_heightMap.Height) / 2f;
            m_position = Vector3.Zero;
            m_quadTree = new QuadTree(-position, m_heightMap, Globals.Camera.ViewMatrix, Globals.Camera.ProjectionMatrix, Game.GraphicsDevice, 1, 5.0f);
            m_quadTree.Texture = m_surface;
        }

        public override void VariableUpdate(GameTime gameTime)
        {
            m_quadTree.View = Globals.Camera.ViewMatrix;
            m_quadTree.Projection = Globals.Camera.ProjectionMatrix;
            m_quadTree.CameraPosition = Globals.Camera.Position;
            m_quadTree.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //m_terrainEffect.Parameters["CameraPosition"].SetValue(Globals.Camera.Position);
            m_terrainEffect.Parameters["View"].SetValue(Globals.Camera.ViewMatrix);
            m_terrainEffect.Parameters["Projection"].SetValue(Globals.Camera.ProjectionMatrix);
            m_terrainEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(m_position));
            ApplyLightToEffect(m_terrainEffect);
            m_quadTree.DrawEffect(gameTime,m_terrainEffect);
        }

        private void BuildNormalMap(ContentManager contentManager)
        {
            //m_terrainEffect.Parameters["NormalMapTexture"].SetValue((Texture)null);

            int width = m_heightMap.Width;

            Effect normalsEffect = contentManager.Load<Effect>("Effects/Terrain/TerrainNormalMap");
            m_normalMap = new RenderTarget2D(Game.GraphicsDevice, width, width, false, SurfaceFormat.Color, DepthFormat.None);

            Game.GraphicsDevice.SetRenderTarget(m_normalMap);
            Game.GraphicsDevice.Clear(Color.White);

            normalsEffect.CurrentTechnique = normalsEffect.Techniques["ComputeNormals"];
            normalsEffect.Parameters["HeightMapTexture"].SetValue(m_heightMap);
            
            float texelWidth = 1f / (float)width;
            normalsEffect.Parameters["TexelWidth"].SetValue(texelWidth);
            normalsEffect.Parameters["normalStrength"].SetValue(8f);
            
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, width, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            //normalsEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            foreach (EffectPass pass in normalsEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_screenQuad.Draw();
            }

            normalsEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);
            Game.GraphicsDevice.SetRenderTarget(null);
            
            //Color[] colorData = new Color[m_normalMap.Width * m_normalMap.Height];
            //m_normalMap.GetData<Color>(colorData);
            //for (int i = 0; i < colorData.Length; ++i)
            //{
            //    if (colorData[i].R != colorData[i].B)
            //    {

            //        int ibreak = 0;
            //    }
            //}
            //int ibreak2 = 0;

            
            //m_terrainEffect.Parameters["NormalMapTexture"].SetValue(m_normalMap);

            //using (Stream stream = File.OpenWrite("c:/tmp/glad-nm.png"))
            //{
            //    m_normalMap.SaveAsJpeg(stream,m_normalMap.Width,m_normalMap.Height);
            //}
        }

        public void ApplyLightToEffect(Effect effect)
        {
            Vector3 ambientLightColor = new Vector3(1f);
            float ambientLightIntensity = 0.5f;
            Vector3 directionalLightColor = new Vector3(1f);
            float directionalLightIntensity = 1f;

            Vector3 specularLightColor = new Vector3(1f);
            float specularLightIntensity = 1f;

            effect.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            effect.Parameters["AmbientLightIntensity"].SetValue(ambientLightIntensity);

            effect.Parameters["DirectionalLightColor"].SetValue(directionalLightColor);
            effect.Parameters["DirectionalLightIntensity"].SetValue(directionalLightIntensity);

            //effect.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            //effect.Parameters["SpecularLightIntensity"].SetValue(specularLightIntensity);

            //effect.Parameters["LightPosition"].SetValue(Globals.Camera.Position);
            Vector3 lightDir = new Vector3(0, 1, -1);
            lightDir.Normalize();
            effect.Parameters["LightDirection"].SetValue(lightDir);
        }

        public void GetHeightAtPoint(ref Vector3 input)
        {
            // find height at x,z
            int x1 = (int)input.X;
            int x2 = (int)(input.X+0.5f);

            int z1 = (int)input.Z;
            int z2 = (int)(input.Z+0.5f);

            input.Y = (m_quadTree.Vertices.GetHeightAtPoint(x1,z1) + 
                m_quadTree.Vertices.GetHeightAtPoint(x2,z1) + 
                m_quadTree.Vertices.GetHeightAtPoint(x1,z2) + 
                m_quadTree.Vertices.GetHeightAtPoint(x2,z2))/4f;
            
            input.Y += 1;
        }

        private QuadTree m_quadTree;
        Texture2D m_heightMap;
        Texture2D m_surface;
        RenderTarget2D m_normalMap;
        Effect m_terrainEffect;
        ScreenQuad m_screenQuad;
        Vector3 m_position;
    }

}
