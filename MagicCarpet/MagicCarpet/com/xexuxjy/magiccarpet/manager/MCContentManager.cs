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

namespace com.xexuxjy.magiccarpet.manager
{
    public class MCContentManager
    {
        public MCContentManager()
        {
            m_contentManager = Globals.Game.Content;
            m_graphicsDevice = Globals.Game.GraphicsDevice;
            m_modelDictionary = new Dictionary<GameObjectType, Model>();
            m_colorMap = new Dictionary<Color, Texture2D>();
            m_textureDictionary = new Dictionary<string, Texture2D>();
            m_effectDictionary = new Dictionary<string, Effect>();
        }

        public void LoadContent()
        {

            m_effectDictionary["Terrain"] = m_contentManager.Load<Effect>("Effects/Terrain/ClipTerrain");
            m_effectDictionary["TerrainNormal"] = m_contentManager.Load<Effect>("Effects/Terrain/TerrainNormalMap");


            m_modelDictionary[GameObjectType.castle] = m_contentManager.Load<Model>("unitcube");
            //m_castleModel = m_contentManager.Load<Model>("Models/Castle/saintriqT3DS");
            //m_castleModel = m_contentManager.Load<Model>("Models/NewCastle/castle3");

            m_modelDictionary[GameObjectType.balloon] = m_contentManager.Load<Model>("unitsphere");

            m_modelDictionary[GameObjectType.manaball] = m_modelDictionary[GameObjectType.balloon];

            m_modelDictionary[GameObjectType.spell] = m_modelDictionary[GameObjectType.balloon]; 

            m_modelDictionary[GameObjectType.monster] = m_contentManager.Load<Model>("unitcone");

            m_modelDictionary[GameObjectType.magician] = m_contentManager.Load<Model>("unitcylinder");


            Model m = m_contentManager.Load<Model>("Models/SkyDome/skydome");
            m_modelDictionary[GameObjectType.skydome] = m;

            m_debugFont = m_contentManager.Load<SpriteFont>("DebugFont8");
            m_basicEffect = new BasicEffect(m_graphicsDevice);
            m_basicEffect.TextureEnabled = true;

            foreach (Model model in m_modelDictionary.Values)
            {
                RemapModel(model, m_basicEffect);
            }


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
        }

        public Model ModelForObjectType(GameObjectType gameObjectType)
        {
            Model model;
            m_modelDictionary.TryGetValue(gameObjectType, out model);
            return model;
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

        public Texture2D GetTexture(ref IndexedVector3 iv3)
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



        public BasicEffect BasicEffect
        {
            get { return m_basicEffect; }
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




        private BasicEffect m_basicEffect;


        //private Model m_spellModel;
        //private Model m_castleModel;
        //private Model m_balloonModel;
        //private Model m_manaBallModel;
        //private Model m_monsterModel;
        //private Model m_magicianModel;
        //private Model m_skyDomeModel;


        private SpriteFont m_debugFont;

        private Dictionary<GameObjectType, Model> m_modelDictionary;
        private Dictionary<String, Texture2D> m_textureDictionary;
        private Dictionary<Color, Texture2D> m_colorMap;
        private Dictionary<String, Effect> m_effectDictionary;

        private ContentManager m_contentManager;
        private GraphicsDevice m_graphicsDevice;
    }
}
