using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.gameobjects;
using Dhpoware;

namespace com.xexuxjy.magiccarpet.renderer
{
    public class DefaultRenderer : DrawableGameComponent
    {
        public DefaultRenderer(Game game,GameObject gameObject)
            : base(game)
        {
            m_gameObject = gameObject;
            game.Components.Add(this);
            
        }

        public override void Initialize()
        {
            m_basicEffect = new BasicEffect(Game.GraphicsDevice);
            if (m_gameObject != null && !String.IsNullOrEmpty(m_gameObject.ModelName))
            {
                m_model = Game.Content.Load<Model>(m_gameObject.ModelName);
                RemapModel(m_model, m_basicEffect);
            }
        }



        public virtual void DrawDebugAxes(GraphicsDevice graphicsDevice)
        {

        }

        public virtual void DrawBoundingBox(GraphicsDevice graphicsDevice)
        {

        }

        public virtual bool ShouldDrawBoundingBox()
        {
            return m_drawBoundingBox;
        }

        public override void Draw(GameTime gameTime)
        {

            ICamera camera = Globals.Camera;

            //Matrix translation = Matrix.CreateTranslation(new Vector3());
            Matrix view = camera.ViewMatrix;
            Matrix world = m_gameObject.WorldTransform;

            Matrix projection = camera.ProjectionMatrix;

            Matrix worldViewProjection = world * view * projection;

            // only one of these should be active.

            DrawEffect(Game.GraphicsDevice, ref view, ref world, ref projection);

            DrawDebugAxes(Game.GraphicsDevice);
            if (ShouldDrawBoundingBox())
            {
                DrawBoundingBox(Game.GraphicsDevice);
            }
        }


        protected virtual void DrawEffect(GraphicsDevice graphicsDevice, ref Matrix view,ref Matrix world,ref Matrix projection)
        {
            if (m_model != null)
            {
                //Matrix scale = Matrix.CreateScale(modelScalingData.scale);
                Matrix[] transforms = new Matrix[m_model.Bones.Count];
                foreach (ModelMesh mesh in m_model.Meshes)
                {
                    m_model.CopyAbsoluteBoneTransformsTo(transforms);
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        m_basicEffect.View = view;
                        m_basicEffect.Projection = projection;
                        m_basicEffect.World = transforms[mesh.ParentBone.Index] * world;
                    }
                    mesh.Draw();
                }
            }
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



        protected GameObject m_gameObject;
        protected Model m_model;
        protected Texture2D m_texture;
        protected bool m_drawBoundingBox;
        protected BasicEffect m_basicEffect;    
    }
}
