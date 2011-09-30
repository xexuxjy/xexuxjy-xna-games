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
        public MCContentManager(Game game)
        {
            m_contentManager = game.Content;
            m_graphicsDevice = game.GraphicsDevice;
            m_dictionary = new Dictionary<GameObjectType, Model>();
        }

        public void LoadContent()
        {
            m_castleModel = m_contentManager.Load<Model>("unitcube");
            m_dictionary[GameObjectType.castle] = m_castleModel;

            m_balloonModel = m_contentManager.Load<Model>("unitsphere");
            m_dictionary[GameObjectType.balloon] = m_balloonModel;

            m_manaBallModel = m_balloonModel;
            m_dictionary[GameObjectType.manaball] = m_manaBallModel;

            m_debugFont = m_contentManager.Load<SpriteFont>("DebugFont8");
            m_basicEffect = new BasicEffect(m_graphicsDevice);


            RemapModel(m_castleModel, m_basicEffect);
            RemapModel(m_balloonModel, m_basicEffect);
            RemapModel(m_manaBallModel, m_basicEffect);
        }

        public Model ModelForObjectType(GameObjectType gameObjectType)
        {
            Model model;
            m_dictionary.TryGetValue(gameObjectType, out model);
            return model;
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


        private Model m_castleModel;
        private Model m_balloonModel;
        private Model m_manaBallModel;

        private SpriteFont m_debugFont;

        private Dictionary<GameObjectType, Model> m_dictionary;
        private ContentManager m_contentManager;
        private GraphicsDevice m_graphicsDevice;
    }
}
