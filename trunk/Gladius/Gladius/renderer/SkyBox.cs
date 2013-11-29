using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.util;
using Microsoft.Xna.Framework.Graphics;
using Gladius.gamestatemanagement.screenmanager;
using Microsoft.Xna.Framework;

namespace Gladius.renderer
{
    public class SkyBox : GameScreenComponent
    {
        public SkyBox(GameScreen gameScreen)
            : base(gameScreen)
        {
            DrawOrder = Globals.SkyBoxDrawOrder;
        }

        public String ModelName
        {
            get;
            set;
        }


        public Vector3 Position
        {
            get;
            set;
        }


        public override void LoadContent()
        {
            m_boxModel = ContentManager.Load<Model>(ModelName);
            m_boxTransforms = new Matrix[m_boxModel.Bones.Count];
            m_boxModel.CopyAbsoluteBoneTransformsTo(m_boxTransforms);
            m_textures = new Texture2D[m_boxModel.Meshes.Count];
            //for (int i = 0; i < textureNames.Length; ++i)
            //{
            //    m_textures[i] = contentManager.Load<Texture2D>(textureNames[i]);
            //}

            //Model newModel = Content.Load<Model>(assetName);
            //textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in m_boxModel.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    m_textures[i++] = currentEffect.Texture;
                }
            }

            foreach (ModelMesh mesh in m_boxModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = meshPart.Effect.Clone();
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, ICamera camera)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            Game.GraphicsDevice.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            Game.GraphicsDevice.DepthStencilState = dss;

            Matrix w = Matrix.CreateTranslation(Position);
            int i = 0;
            foreach (ModelMesh mesh in m_boxModel.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = w * m_boxTransforms[mesh.ParentBone.Index];
                    currentEffect.World = worldMatrix;
                    currentEffect.View = camera.View;
                    currentEffect.Projection = camera.Projection;
                    currentEffect.Texture = m_textures[i++];
                    currentEffect.TextureEnabled = true;
                    //currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    //currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    //currentEffect.Parameters["xView"].SetValue(camera.View);
                    //currentEffect.Parameters["xProjection"].SetValue(camera.Projection);
                    //currentEffect.Parameters["xTexture"].SetValue(m_textures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            Game.GraphicsDevice.DepthStencilState = dss;
        }


        private Model m_boxModel;
        private Texture2D[] m_textures;
        private Matrix[] m_boxTransforms;
    }
}
