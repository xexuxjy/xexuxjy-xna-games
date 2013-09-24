using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using Gladius.util;
using GameStateManagement;
using Gladius.renderer;

namespace Gladius.modes.overland
{
    public class TownManager : GameScreenComponent
    {
        public TownManager(GameScreen gamescreen,Terrain terrain)
            : base(gamescreen)
        {
            UpdateFrequency = 10;
            m_terrain = terrain;
        }

        public override void VariableUpdate(GameTime gameTime)
        {

        }

        public Town NearestTown(Vector3 positions)
        {
            float checkRadius = 2f;
            BoundingSphere bs = new BoundingSphere(positions,checkRadius);
            return m_towns.First(x => x.Bounds.Intersects(bs));
        }

        public override void Draw(GameTime gameTime)
        {
            Draw(Globals.Camera, Game.GraphicsDevice,gameTime);
        }

        public void Draw(ICamera camera, GraphicsDevice graphicsDevice,GameTime gmmeTime)
        {
            m_basicEffect.Projection = camera.Projection;
            m_basicEffect.View = camera.View;

            foreach (Town town in m_towns)
            {
                //if (camera.BoundingFrustum.Contains(town.Bounds) != ContainmentType.Disjoint)
                {
                    m_basicEffect.World = Matrix.CreateTranslation(town.Bounds.Center);
                    foreach (ModelMesh mesh in m_townModel.Meshes)
                    {
                        mesh.Draw();
                    }
                }
            }
        }

        public void LoadContent()
        {
            m_towns.Clear();
            using (StreamReader sr = new StreamReader(TitleContainer.OpenStream("Content/OverlandData/TownData.xml")))
            {
                String result = sr.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                XmlNodeList nodes = doc.SelectNodes("//Town");
                foreach (XmlNode node in nodes)
                {
                    Town town = new Town(node as XmlElement);
                    //Vector3 temp = town.Bounds.Center;
                    m_terrain.GetHeightAtPoint(ref town.Bounds.Center);
                    m_towns.Add(town);
                }
            }

            m_basicEffect = new BasicEffect(Game.GraphicsDevice);
            m_basicEffect.TextureEnabled = true;
            m_basicEffect.Texture = Globals.GlobalContentManager.GetColourTexture(Color.Pink);

            m_townModel = ContentManager.Load<Model>("UnitCube");
            Globals.RemapModel(m_townModel, m_basicEffect);


        }


        private List<Town> m_towns = new List<Town>();
        private Model m_townModel;
        private BasicEffect m_basicEffect;
        private Terrain m_terrain;
    }



    public class Town
    {
        public String Name;
        public BoundingSphere Bounds;
        public Model Model;

        public Town(XmlElement node)
        {
            Name = node.Attributes["name"].Value;
            float x = float.Parse(node.Attributes["x"].Value);
            float y = float.Parse(node.Attributes["y"].Value);
            float z = float.Parse(node.Attributes["z"].Value);
            float r = float.Parse(node.Attributes["r"].Value);
            Bounds = new BoundingSphere(new Vector3(x, y, z), r);
        }
    }


}
