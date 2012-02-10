using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using BulletXNA.LinearMath;

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

            m_effectDictionary["Terrain"] = m_contentManager.Load<Effect>("Effects\\Terrain\\ClipTerrain");
            m_effectDictionary["TerrainNormal"] = m_contentManager.Load<Effect>("Effects\\Terrain\\TerrainNormalMap");

            m_textureDictionary["TerrainBase"] = m_contentManager.Load<Texture2D>("Textures\\Terrain\\base");
            m_textureDictionary["TerrainNoise"] = m_contentManager.Load<Texture2D>("Textures\\Terrain\\noise");
            
            
            m_castleModel = m_contentManager.Load<Model>("unitcube");
            //m_castleModel = m_contentManager.Load<Model>("Models/Castle/saintriqT3DS");
            m_modelDictionary[GameObjectType.castle] = m_castleModel;

            m_balloonModel = m_contentManager.Load<Model>("unitsphere");
            m_modelDictionary[GameObjectType.balloon] = m_balloonModel;

            m_manaBallModel = m_balloonModel;
            m_modelDictionary[GameObjectType.manaball] = m_manaBallModel;

            m_spellModel = m_balloonModel;
            m_modelDictionary[GameObjectType.spell] = m_spellModel;

            m_monsterModel = m_contentManager.Load<Model>("unitcone");
            m_modelDictionary[GameObjectType.monster] = m_monsterModel;

            m_magicianModel = m_contentManager.Load<Model>("unitcylinder");
            m_modelDictionary[GameObjectType.magician] = m_magicianModel;


            m_debugFont = m_contentManager.Load<SpriteFont>("DebugFont8");
            m_basicEffect = new BasicEffect(m_graphicsDevice);
            m_basicEffect.TextureEnabled = true;

            RemapModel(m_castleModel, m_basicEffect);
            RemapModel(m_balloonModel, m_basicEffect);
            RemapModel(m_manaBallModel, m_basicEffect);
            RemapModel(m_spellModel, m_basicEffect);
            RemapModel(m_magicianModel, m_basicEffect);
            RemapModel(m_monsterModel, m_basicEffect);


            m_textureDictionary["MapSpriteAtlas"] = m_contentManager.Load<Texture2D>("textures/ui/MapTextureAtlas");



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



        private BasicEffect m_basicEffect;


        private Model m_spellModel;
        private Model m_castleModel;
        private Model m_balloonModel;
        private Model m_manaBallModel;
        private Model m_monsterModel;
        private Model m_magicianModel;


        private SpriteFont m_debugFont;

        private Dictionary<GameObjectType, Model> m_modelDictionary;
        private Dictionary<String, Texture2D> m_textureDictionary;
        private Dictionary<Color, Texture2D> m_colorMap;
        private Dictionary<String, Effect> m_effectDictionary;

        private ContentManager m_contentManager;
        private GraphicsDevice m_graphicsDevice;
    }
}
