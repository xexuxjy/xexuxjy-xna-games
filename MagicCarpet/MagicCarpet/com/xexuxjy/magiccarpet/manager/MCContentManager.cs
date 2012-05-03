using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.spells;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.renderer;

namespace com.xexuxjy.magiccarpet.manager
{
    public class MCContentManager
    {
        public MCContentManager()
        {
            m_contentManager = Globals.Game.Content;
            m_graphicsDevice = Globals.Game.GraphicsDevice;
            m_modelDictionary = new Dictionary<String, ModelHelperData>();
            m_colorMap = new Dictionary<Color, Texture2D>();
            m_textureDictionary = new Dictionary<string, Texture2D>();
            m_effectDictionary = new Dictionary<string, Effect>();
        }

        public void LoadContent()
        {



            m_effectDictionary["Terrain"] = m_contentManager.Load<Effect>("Effects/Terrain/ClipTerrain");
            m_effectDictionary["TerrainNormal"] = m_contentManager.Load<Effect>("Effects/Terrain/TerrainNormalMap");
            m_effectDictionary["OwnerColour"] = m_contentManager.Load<Effect>("Effects/OwnerColour");
            m_effectDictionary["Simple"] = m_contentManager.Load<Effect>("Effects/SimpleEffect");
            m_effectDictionary["SkyDome"] = m_contentManager.Load<Effect>("Effects/Skydome/Skydome");


            LoadModel("UnitCube", "Models/SimpleShapes/unitcube");

            
            LoadModel(GameObjectType.castle.ToString(), "Models/SimpleShapes/unitcube");
            LoadModel("TerrainWalls","Models/Terrain/TerrainWalls");

            //m_modelDictionary[GameObjectType.castle] = m_contentManager.Load<Model>("Models/NewCastle/castle_tower");

            //m_castleModel = m_contentManager.Load<Model>("Models/Castle/saintriqT3DS");
            //m_castleModel = m_contentManager.Load<Model>("Models/NewCastle/castle3");

            LoadModel(GameObjectType.balloon.ToString(),"Models/SimpleShapes/unitsphere");

            LoadModel(GameObjectType.manaball.ToString(),"Models/SimpleShapes/unitsphere");

            LoadModel(GameObjectType.spell.ToString(), "Models/SimpleShapes/unitsphere");

            LoadModel(GameObjectType.monster.ToString(),"Models/SimpleShapes/unitcone");

            //m_modelDictionary[GameObjectType.magician] = m_contentManager.Load<Model>("unitcylinder");
            LoadModel(GameObjectType.magician.ToString(),"Models/Magician/magician");
            LoadModel("CastleTower", "Models/NewCastle/castle_tower");
            LoadModel("CastleWall", "Models/NewCastle/castle_wall");

            LoadModel("SkyDome","Models/SkyDome/skydome");

            m_debugFont = m_contentManager.Load<SpriteFont>("DebugFont8");

            Effect simpleEffect = GetEffect("Simple");
            foreach (ModelHelperData modelHelperData in m_modelDictionary.Values)
            {
                RemapModel(modelHelperData.m_model, simpleEffect);
            }


            RemapModel(m_modelDictionary["TerrainWalls"].m_model, m_effectDictionary["Terrain"]);
            RemapModel(m_modelDictionary["SkyDome"].m_model, m_effectDictionary["SkyDome"]);


            m_textureDictionary["MiniMapAtlas"] = m_contentManager.Load<Texture2D>("textures/ui/MiniMapAtlas");
            m_textureDictionary["PlayerStatsFrame"] = m_contentManager.Load<Texture2D>("textures/ui/PlayerFrame");
            m_textureDictionary["MiniMapFrame"] = m_contentManager.Load<Texture2D>("textures/ui/MiniMapFrame");
            m_textureDictionary["SpellAtlas"] = m_contentManager.Load<Texture2D>("textures/ui/SpellAtlas");
            m_textureDictionary["SpellSelector"] = m_contentManager.Load<Texture2D>("textures/ui/SpellSelector");
            m_textureDictionary["SkyDome"] = m_contentManager.Load<Texture2D>("Models/SkyDome/SkyDomeTexture");

            m_textureDictionary["TerrainBase"] = m_contentManager.Load<Texture2D>("Textures/Terrain/Base");
            m_textureDictionary["TerrainNoise"] = m_contentManager.Load<Texture2D>("Textures/Terrain/Noise");
            m_textureDictionary["TerrainHeightMap"] = new Texture2D(Globals.GraphicsDevice, Globals.WorldWidth + 1, Globals.WorldWidth + 1, false, SurfaceFormat.Single);
            m_textureDictionary["TerrainNormalMap"] = new RenderTarget2D(Globals.GraphicsDevice, Globals.WorldWidth + 1, Globals.WorldWidth + 1, false, SurfaceFormat.Color, DepthFormat.None);
            m_textureDictionary["TreeBillboard"] = m_contentManager.Load<Texture2D>("Textures/Terrain/TreeBillBoard");
            m_textureDictionary["CastleTower"] = m_contentManager.Load<Texture2D>("Models/NewCastle/wallstone");

            m_textureDictionary["Carpet2"] = m_contentManager.Load<Texture2D>("Textures/Magician/Carpet2");
            m_textureDictionary["Flag"] = m_contentManager.Load<Texture2D>("Textures/flag");
        
        }

        private void LoadModel(String modelKey, String path)
        {
            Model m = m_contentManager.Load<Model>(path);
            BoundingBox bb = UpdateBoundingBox(m, Matrix.Identity);
            ModelHelperData modelHelperData = new ModelHelperData(m, bb);
            m_modelDictionary[modelKey] = modelHelperData;
        }


        public Model GetModelForObjectType(GameObjectType gameObjectType)
        {
            ModelHelperData modelHelperData;
            m_modelDictionary.TryGetValue(gameObjectType.ToString(), out modelHelperData);
            return modelHelperData != null ? modelHelperData.m_model : null;
        }

        public Model GetModelForName(String name)
        {
            ModelHelperData modelHelperData;
            m_modelDictionary.TryGetValue(name, out modelHelperData);
            return modelHelperData != null ? modelHelperData.m_model : null;
        }

        public ModelHelperData GetModelHelperData(String name)
        {
            ModelHelperData modelHelperData;
            m_modelDictionary.TryGetValue(name, out modelHelperData);
            return modelHelperData;
        }

        public Texture2D GetTexture(Color color)
        {
            if (!m_colorMap.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(m_graphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                colorData[0] = color;
                newTexture.SetData(colorData);
                m_colorMap[color] = newTexture;
            }
            return m_colorMap[color];
        
        
        
        
        }

        public Texture2D GetTexture(ref Vector3 iv3)
        {
            Color color = new Color(iv3);
            return GetTexture(color);
        }

        public Texture2D GetTexture(String key)
        {
            Debug.Assert(m_textureDictionary.ContainsKey(key));
            // if it doesn't we should log it?? then replace with dummy pink texture...
            return m_textureDictionary[key];
        }

        public Effect GetEffect(String key)
        {
            return m_effectDictionary[key];
        }

        
        
        public void Initialize()
        {
            LoadContent();
        }

        private void RemapModel(Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }

        public SpriteFont DebugFont
        {
            get{return m_debugFont;}
        }

        public SpriteFont EventWindowFont
        {
            get { return m_debugFont; }
        }



        public Rectangle? MiniMapSpritePositionForGameObject(GameObject gameObject)
        {
            int spriteWidth = 16;

            int xpos = 0;
            int ypos = 0;
            bool found = false;
            Rectangle? result = null;
            switch (gameObject.GameObjectType)
            {
                case (GameObjectType.balloon):
                    {
                        xpos = 0;
                        ypos = 0;
                        found = true;
                        break;
                    }
                case (GameObjectType.castle):
                    {
                        xpos = 1;
                        ypos = 0;
                        found = true;
                        break;
                    }
                case (GameObjectType.manaball):
                    {
                        xpos = 2;
                        ypos = 0;
                        found = true;
                        break;
                    }
                default:
                    {
                        break;
                    }

            }

            if (found)
            {
                result = new Rectangle(xpos * spriteWidth, ypos * spriteWidth, spriteWidth, spriteWidth);
            }
            return result;
        }


        public Rectangle? SpritePositionForSpellType(SpellType spellType)
        {
            int spriteWidth = 64;

            int xpos = 0;
            int ypos = 0;
            bool found = false;
            Rectangle? result = null;
            switch (spellType)
            {
                case (SpellType.Raise):
                    {
                        xpos = 0;
                        ypos = 0;
                        found = true;
                        break;
                    }
                case (SpellType.Lower):
                    {
                        xpos = 1;
                        ypos = 0;
                        found = true;
                        break;
                    }
                case (SpellType.Fireball):
                    {
                        xpos = 2;
                        ypos = 0;
                        found = true;
                        break;
                    }
                case (SpellType.Heal):
                    {
                        xpos = 3;
                        ypos = 0;
                        found = true;
                        break;
                    }
                case (SpellType.Castle):
                    {
                        xpos = 0;
                        ypos = 1;
                        found = true;
                        break;
                    }
                case (SpellType.Convert):
                    {
                        xpos = 1;
                        ypos = 1;
                        found = true;
                        break;
                    }

                default:
                    {
                        break;
                    }

            }

            if (found)
            {
                result = new Rectangle(xpos * spriteWidth, ypos * spriteWidth, spriteWidth, spriteWidth);
            }
            return result;
        }


        public void ApplyCommonEffectParameters(Effect effect)
        {
            LightManager.ApplyLightToEffect(effect);

            effect.Parameters["CameraPosition"].SetValue(Globals.Camera.Position);
            effect.Parameters["ViewMatrix"].SetValue(Globals.Camera.ViewMatrix);
            effect.Parameters["ProjMatrix"].SetValue(Globals.Camera.ProjectionMatrix);

            effect.Parameters["FogEnabled"].SetValue(true);
            effect.Parameters["FogStart"].SetValue(20);
            effect.Parameters["FogEnd"].SetValue(200);
            effect.Parameters["EdgeFog"].SetValue(5);
            effect.Parameters["WorldWidth"].SetValue(Globals.WorldWidth);

            effect.Parameters["UnassignedPlayerColor"].SetValue(Globals.unassignedPlayerColour.ToVector3());

        }


        protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // swap yz from blender
            min = new Vector3(min.X, min.Z, min.Y);
            max = new Vector3(max.X, max.Z, max.Y);

            // Create and return bounding box
            return new BoundingBox(min, max);
        }


        private SpriteFont m_debugFont;

        private Dictionary<String, ModelHelperData> m_modelDictionary;
        private Dictionary<String, Texture2D> m_textureDictionary;
        private Dictionary<Color, Texture2D> m_colorMap;
        private Dictionary<String, Effect> m_effectDictionary;

        private ContentManager m_contentManager;
        private GraphicsDevice m_graphicsDevice;
    }


    public class ModelHelperData
    {
        public ModelHelperData(Model model, BoundingBox boundingBox)
        {
            m_model = model;
            m_boundingBox = boundingBox;
        }
        public Model m_model;
        public BoundingBox m_boundingBox;
    }



    public struct PosOnlyVertex : IVertexType
    {

        public PosOnlyVertex(Vector2 v)
        {
            Position = v;
        }

        public Vector2 Position;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };

    public struct VertexPositionScaleTexture : IVertexType
    {

        public VertexPositionScaleTexture(Vector3 v, float s, Vector2 uv)
        {
            Position = v;
            Scale = s;
            UV = uv;
        }

        

        public Vector3 Position;
        public float Scale;
        public Vector2 UV;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };





}
