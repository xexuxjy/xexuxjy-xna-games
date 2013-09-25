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
using Gladius.gamestatemanagement.screens;

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

        public Terrain(OverlandScreen gameScreen)
            : base(gameScreen)
        {
            m_overlandScreen = gameScreen;
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
            m_quadTree = new QuadTree(-position, m_heightMap, Globals.Camera.View, Globals.Camera.Projection, Game.GraphicsDevice, 1, 5.0f);
            m_quadTree.Texture = m_surface;
        
            m_lightingSpans.Add(new LightingSpan(0,4,Color.Black,Color.OrangeRed));
            m_lightingSpans.Add(new LightingSpan(4,7,Color.OrangeRed,Color.White));
            m_lightingSpans.Add(new LightingSpan(7,19,Color.White,Color.White));
            m_lightingSpans.Add(new LightingSpan(19,21,Color.White,Color.OrangeRed));
            m_lightingSpans.Add(new LightingSpan(21,24,Color.OrangeRed,Color.Black));
        
        }

        public override void VariableUpdate(GameTime gameTime)
        {
            m_quadTree.View = Globals.Camera.View;
            m_quadTree.Projection = Globals.Camera.Projection;
            m_quadTree.CameraPosition = Globals.Camera.Position;
            m_quadTree.Update(gameTime);
            UpdateTimeOfDay(gameTime.ElapsedGameTime.TotalSeconds);

        }

        public override void Draw(GameTime gameTime)
        {
            //m_terrainEffect.Parameters["CameraPosition"].SetValue(Globals.Camera.Position);
            m_terrainEffect.Parameters["View"].SetValue(Globals.Camera.View);
            m_terrainEffect.Parameters["Projection"].SetValue(Globals.Camera.Projection);
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


            m_terrainEffect.Parameters["NormalMapTexture"].SetValue(m_normalMap);

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

            effect.Parameters["AmbientLightColor"].SetValue(m_ambientLightColor);
            effect.Parameters["AmbientLightIntensity"].SetValue(ambientLightIntensity);

            //effect.Parameters["DirectionalLightColor"].SetValue(directionalLightColor);
            //effect.Parameters["DirectionalLightIntensity"].SetValue(directionalLightIntensity);
            effect.Parameters["PointLightPosition"].SetValue(m_overlandScreen.Party.Position);
            effect.Parameters["PointLightRadius"].SetValue(0.5f);
            effect.Parameters["PointLightColor"].SetValue(Vector3.One);


            //effect.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            //effect.Parameters["SpecularLightIntensity"].SetValue(specularLightIntensity);

            //effect.Parameters["LightPosition"].SetValue(Globals.Camera.Position);
            Vector3 lightDir = new Vector3(0, 1, -1);
            lightDir.Normalize();
            //effect.Parameters["LightDirection"].SetValue(lightDir);
        }

        public void UpdateTimeOfDay(double seconds)
        {
            m_timeOfDay += (seconds * m_timeMultiplier);



            double hourMultiplier = 1000 * 60 * 60;
            double twentyFourHours =  hourMultiplier * 24;

            if (m_timeOfDay > twentyFourHours)
            {
                m_timeOfDay = 0;
            }


            double midnight = 0f;
            double threeAm = 3 * hourMultiplier;

            // 00:00 -> 04:00 black.
            // 04:00 -> 06:00 black->red / orange
            // 06:00 -> 07:00 red/orage-> white

            // 07:00 -> 19:00 white

            // 19:00->21:00 white ->red/orange
            // 21:00-> 00:00 red/orange -> black


            foreach(LightingSpan span in m_lightingSpans)
            {
                if(m_timeOfDay > span.startTime && m_timeOfDay < span.endTime)
                {
                    double lerpVal = (m_timeOfDay - span.startTime) / (span.endTime - span.startTime);
                    m_ambientLightColor = Vector3.Lerp(span.startColor,span.endColor,(float)lerpVal);
                    break;
                }
            }

        }

        public double Lerp(double val1, double val2, double amount)
        {
            if (amount <= 0)
            {
                return val1;
            }
            if (amount >= 1)
            {
                return val2;
            }
            return val1 + ((val2-val1) * amount);
        }


        public void GetHeightAtPoint(ref Vector3 input)
        {
            // find height at x,z
            int x1 = (int)input.X;
            int x2 = (int)(input.X+1f);

            int z1 = (int)input.Z;
            int z2 = (int)(input.Z+1f);

            float xrem = input.X%1;
            float zrem = input.Z % 1;

            float tl = m_quadTree.Vertices.GetHeightAtPoint(x1, z1);
            float tr = m_quadTree.Vertices.GetHeightAtPoint(x2, z1);
            float bl = m_quadTree.Vertices.GetHeightAtPoint(x1, z2);
            float br = m_quadTree.Vertices.GetHeightAtPoint(x2, z2);

            float val1 = MathHelper.Lerp(tl,tr,xrem);
            float val2 = MathHelper.Lerp(tl,bl,zrem);
            float val3 = MathHelper.Lerp(bl,br,xrem);
            float val4 = MathHelper.Lerp(tr,br,zrem);

            float val5 = MathHelper.Lerp(val1,val3,zrem);
            float val6 = MathHelper.Lerp(val2,val4,xrem);

            input.Y = val5;


            //float xdiff = tr - tl;
            //float zdiff = 

            //float xDiff = Math.lerp((,tl,tr);

            //input.Y = (m_quadTree.Vertices.GetHeightAtPoint(x1,z1) + 
            //    m_quadTree.Vertices.GetHeightAtPoint(x2,z1) + 
            //    m_quadTree.Vertices.GetHeightAtPoint(x1,z2) + 
            //    m_quadTree.Vertices.GetHeightAtPoint(x2,z2))/4f;
            
            //input.Y += 1;
        }

        public Color AmbientLightColor
        {
            get { return new Color(m_ambientLightColor); }
        }


        double m_timeOfDay;
        // 1 second = 1 hour
        double m_timeMultiplier = 1000 * 60 * 60;

        Vector3 m_ambientLightColor;
        float m_ambientLightIntensity;

        OverlandScreen m_overlandScreen;
        QuadTree m_quadTree;
        Texture2D m_heightMap;
        Texture2D m_surface;
        RenderTarget2D m_normalMap;
        Effect m_terrainEffect;
        ScreenQuad m_screenQuad;
        Vector3 m_position;
        List<LightingSpan> m_lightingSpans = new List<LightingSpan>();
    }

    struct LightingSpan
    {
        public LightingSpan(int startHour,int endHour,Color start,Color end)
        {
            startTime = (startHour * (1000 * 60 * 60));
            endTime = (endHour * (1000 * 60 * 60));
            startColor = start.ToVector3();
            endColor = end.ToVector3();
        }

        public double startTime;
        public double endTime;
        public Vector3 startColor;
        public Vector3 endColor;
    };

}
