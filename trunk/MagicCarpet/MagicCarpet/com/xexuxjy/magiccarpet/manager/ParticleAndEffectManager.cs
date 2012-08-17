using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BulletXNA.LinearMath;
using ProjectMercury;
using ProjectMercury.Renderers;
using System.Diagnostics;
using ProjectMercury.Proxies;

namespace com.xexuxjy.magiccarpet.manager
{
    public class ParticleAndEffectManager : EmptyGameObject
    {
        public ParticleAndEffectManager()
            : base(GameObjectType.manager)

        {
        }

        public override void Initialize()
        {
            base.Initialize();
            
            m_particleRenderer = new QuadRenderer(10000);
            m_particleRenderer.GraphicsDeviceService = Globals.GraphicsDeviceManager;
            m_particleRenderer.LoadContent(Globals.Game.Content);
            DrawOrder = Globals.PARTICLE_DRAW_ORDER;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ParticleEffect LoadParticleEffect(String particleEffectName)
        {
            ParticleEffect particleEffect = null;
            if (!m_particleEffects.ContainsKey(particleEffectName))
            {
                particleEffect = Globals.MCContentManager.LoadParticleEffect(particleEffectName);
                if (particleEffect != null)
                {
                    m_particleEffects[particleEffectName] = particleEffect;
                }
            }
            else
            {
                particleEffect = m_particleEffects[particleEffectName];
            }
            return particleEffect;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            float elapsedTimeSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (ParticleEffect particleEffect in m_particleEffects.Values)
            {
                particleEffect.Update(elapsedTimeSeconds);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public void TriggerParticleEffect(String effectName,Vector3 position)
        //{
        //    // FIXME - don't trigger effects outside of view?
        //    ParticleEffect particleEffect = null;
        //    if (m_particleEffects.TryGetValue(effectName, out particleEffectProxy))
        //    {
        //        particleEffectProxy.Trigger(position);
        //    }
        //    else
        //    {
        //        Debug.Assert(false);
        //    }

        //}
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public TempGraphicHolder AddTempGraphicHolder(GameObject ownerObject, Model model, Texture2D texture, Texture2D normalTexture, Matrix transform)
        {

            TempGraphicHolder tempHolder = new TempGraphicHolder(ownerObject, model, texture, normalTexture, transform);
            m_tempGraphicHolders.Add(tempHolder);
            return tempHolder;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ReleaseTempGraphicHolder(TempGraphicHolder tempHolder)
        {
            m_tempGraphicHolders.Remove(tempHolder);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        static Matrix[] s_boneTransforms = new Matrix[10];


        public override void Draw(GameTime gameTime)
        {
            DrawEffects(Matrix.Identity,Globals.Camera.ViewMatrix, Globals.Camera.ProjectionMatrix);
        }


        public void DrawEffects(Matrix world, Matrix view, Matrix projection)
        {
            // draw particles


            Vector3 cameraPos = Globals.Camera.Position;

            if (m_particleRenderer != null)
            {
                foreach (ParticleEffect particleEffect in m_particleEffects.Values)
                {
                    m_particleRenderer.RenderEffect(particleEffect, ref world, ref view, ref projection, ref cameraPos);
                }
            }


            // draw effects

            Globals.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            foreach (TempGraphicHolder holder in m_tempGraphicHolders)
            {
                if (holder.m_active && holder.m_model != null)
                {
                    //if (Globals.s_currentCameraFrustrum.Contains(holder.m_boundingBox) != ContainmentType.Disjoint)
                    {
                        foreach (ModelMesh mesh in holder.m_model.Meshes)
                        {
                            holder.m_model.CopyAbsoluteBoneTransformsTo(s_boneTransforms);
                            foreach (Effect effect in mesh.Effects)
                            {
                                effect.CurrentTechnique = effect.Techniques["SimpleTechnique"];
                                Globals.MCContentManager.ApplyCommonEffectParameters(effect);

                                Matrix owner = holder.m_owner != null ? holder.m_owner.WorldTransform : Matrix.Identity;
                                Matrix result = owner * holder.m_transform * world;

                                effect.Parameters["WorldMatrix"].SetValue(s_boneTransforms[mesh.ParentBone.Index] * result);
                                Texture2D texture = holder.m_texture;
                                if (texture != null)
                                {
                                    effect.Parameters["Texture"].SetValue(texture);
                                }

                                Texture2D normalTexture = holder.m_normalTexture;
                                if (texture != null)
                                {
                                    effect.Parameters["NormalTexture"].SetValue(normalTexture);
                                }
                            }
                            mesh.Draw();
                        }
                    }
                }
            }
            Globals.GraphicsDevice.BlendState = BlendState.Opaque;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private AbstractRenderer m_particleRenderer;
        public ObjectArray<TempGraphicHolder> m_tempGraphicHolders = new ObjectArray<TempGraphicHolder>();
        public Dictionary<String, ParticleEffect> m_particleEffects = new Dictionary<String, ParticleEffect>();
    }
}
