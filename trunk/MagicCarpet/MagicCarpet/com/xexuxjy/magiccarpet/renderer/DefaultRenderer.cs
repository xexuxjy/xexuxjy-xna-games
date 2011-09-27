//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using com.xexuxjy.magiccarpet.gameobjects;
//using Dhpoware;

//namespace com.xexuxjy.magiccarpet.renderer
//{
//    public class DefaultRenderer : DrawableGameComponent
//    {
//        public DefaultRenderer(Game game,GameObject gameObject)
//            : base(game)
//        {
//            m_gameObject = gameObject;
//            game.Components.Add(this);
            
//        }

//        public override void Initialize()
//        {
//            m_basicEffect = new BasicEffect(Game.GraphicsDevice);
//            if (m_gameObject != null && !String.IsNullOrEmpty(m_gameObject.ModelName))
//            {
//                m_model = Game.Content.Load<Model>(m_gameObject.ModelName);
//                RemapModel(m_model, m_basicEffect);
//            }
//        }




//        public virtual bool ShouldDrawBoundingBox()
//        {
//            return m_drawBoundingBox;
//        }


//        private void RemapModel(Model model, Effect effect)
//        {
//            foreach (ModelMesh mesh in model.Meshes)
//            {
//                foreach (ModelMeshPart part in mesh.MeshParts)
//                {
//                    part.Effect = effect;
//                }
//            }
//        }



//        protected GameObject m_gameObject;
//        protected Model m_model;
//        protected Texture2D m_texture;
//        protected bool m_drawBoundingBox;
//        protected BasicEffect m_basicEffect;    
//    }
//}
