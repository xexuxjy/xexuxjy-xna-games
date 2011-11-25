using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.manager
{
    public class MCContentManager
    {
        public MCContentManager()
        {
            m_contentManager = Globals.Game.Content;
            m_graphicsDevice = Globals.Game.GraphicsDevice;
            m_dictionary = new Dictionary<GameObjectType, Model>();
            m_colorMap = new Dictionary<Vector3, Texture2D>();
        }

        public void LoadContent()
        {
            m_castleModel = m_contentManager.Load<Model>("unitcube");
            m_dictionary[GameObjectType.castle] = m_castleModel;

            m_balloonModel = m_contentManager.Load<Model>("unitsphere");
            m_dictionary[GameObjectType.balloon] = m_balloonModel;

            m_manaBallModel = m_balloonModel;
            m_dictionary[GameObjectType.manaball] = m_manaBallModel;

            m_spellModel = m_balloonModel;
            m_dictionary[GameObjectType.spell] = m_spellModel;

            m_monsterModel = m_contentManager.Load<Model>("unitcone");
            m_dictionary[GameObjectType.monster] = m_monsterModel;

            m_magicianModel = m_contentManager.Load<Model>("unitcylinder");
            m_dictionary[GameObjectType.magician] = m_magicianModel;


            m_debugFont = m_contentManager.Load<SpriteFont>("DebugFont8");
            m_basicEffect = new BasicEffect(m_graphicsDevice);
            m_basicEffect.TextureEnabled = true;

            RemapModel(m_castleModel, m_basicEffect);
            RemapModel(m_balloonModel, m_basicEffect);
            RemapModel(m_manaBallModel, m_basicEffect);
            RemapModel(m_spellModel, m_basicEffect);
            RemapModel(m_magicianModel, m_basicEffect);
            RemapModel(m_monsterModel, m_basicEffect);


        }

        public Model ModelForObjectType(GameObjectType gameObjectType)
        {
            Model model;
            m_dictionary.TryGetValue(gameObjectType, out model);
            return model;
        }

        public Texture2D GetTexture(ref Vector3 color)
        {
            if (!m_colorMap.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(m_graphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                newTexture.GetData<Color>(colorData);
                colorData[0] = new Color(color);
                newTexture.SetData(colorData);
                m_colorMap[color] = newTexture;
            }
            return m_colorMap[color];
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


        private BasicEffect m_basicEffect;


        private Model m_spellModel;
        private Model m_castleModel;
        private Model m_balloonModel;
        private Model m_manaBallModel;
        private Model m_monsterModel;
        private Model m_magicianModel;


        private SpriteFont m_debugFont;

        private Dictionary<GameObjectType, Model> m_dictionary;
        private Dictionary<Vector3, Texture2D> m_colorMap;

        private ContentManager m_contentManager;
        private GraphicsDevice m_graphicsDevice;
    }
}
