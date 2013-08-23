
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;
using Gladius.actors;
using Microsoft.Xna.Framework.Content;
using System;
using xexuxjy.Gladius.util;
using CpuSkinningDataTypes;
using Gladius.util;
using System.Collections.Generic;

namespace Gladius.renderer.animation
{
    public class AnimatedModel 
    {
        public AnimatedModel(BaseActor baseActor)
        {
            m_baseActor = baseActor;
        }

        public void Update(GameTime gameTime)
        {
            m_animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            // send notification if we haven't already.
            if (m_animationPlayer.ClipComplete && m_currentAnimationEnum != AnimationEnum.None)
            {
                if (OnAnimationStopped != null)
                {
                    OnAnimationStopped(m_currentAnimationEnum);
                }
                m_currentAnimationEnum = AnimationEnum.None;
            }

            // fall back to idle.
            if (m_currentAnimationEnum == AnimationEnum.None)
            {
                PlayAnimation(AnimationEnum.Idle);
            }

        }

        public void LoadContent(ContentManager content)
        {
            // Load the model.
            m_model = content.Load<Model>(m_baseActor.ModelName);
            BoundingSphere actorBs = m_model.Meshes[0].BoundingSphere;
            m_baseActorScale = new Vector3(1f / actorBs.Radius);

            // Look up our custom skinning information.
            SkinningData skinningData = m_model.Tag as SkinningData;

            if (skinningData == null)
            {
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");
            }
            // Create an animation player, and start decoding an animation clip.
            m_animationPlayer = new AnimationPlayer(skinningData);
            m_skinningData = skinningData;

            m_clipNameDictionary[AnimationEnum.Idle] = "Walk";
            m_clipNameDictionary[AnimationEnum.Walk] = "Walk";
            m_clipNameDictionary[AnimationEnum.Attack1] = "Attack1";
            
        }


        public void Draw(GraphicsDevice device, ICamera camera, GameTime gameTime)
        {
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            
            Matrix[] bones = m_animationPlayer.SkinTransforms;

            Matrix world = Matrix.CreateScale(m_baseActorScale) * Matrix.CreateFromQuaternion(m_baseActor.Rotation) * Matrix.CreateTranslation(m_baseActor.Position);
            // Render the skinned mesh.
            foreach (ModelMesh mesh in m_model.Meshes)
            {
                Matrix boneWorld = bones[mesh.ParentBone.Index] * world;
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.World = boneWorld;
                    effect.SetBoneTransforms(bones);

                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
        }

        public void PlayAnimation(AnimationEnum animationEnum,bool loopClip = true)
        {
            String clipName;
            if(m_clipNameDictionary.TryGetValue(animationEnum,out clipName))
            {
                m_currentAnimationEnum = animationEnum;
                m_currentAnimationClip = m_skinningData.AnimationClips[clipName];
                if (m_currentAnimationClip != null)
                {
                    Globals.EventLogger.LogEvent(EventTypes.Animation, String.Format("PlayAnimation [{0}] [{1}] [{2}][{3}]", m_baseActor.DebugName,animationEnum, clipName,loopClip));
                    m_animationPlayer.StartClip(m_currentAnimationClip,loopClip);
                    if (OnAnimationStarted != null)
                    {
                        OnAnimationStarted(m_currentAnimationEnum);
                    }
                }
            }
            else
            {
                Globals.EventLogger.LogEvent(EventTypes.Animation, String.Format("PlayAnimation FailedNoMatch [{0}] [{1}]", m_baseActor.DebugName, animationEnum));
            }
        }


        public delegate void AnimationStarted(AnimationEnum anim);
        public delegate void AnimationStopped(AnimationEnum anim);
        public event AnimationStarted OnAnimationStarted;
        public event AnimationStopped OnAnimationStopped;




        private Dictionary<AnimationEnum, String> m_clipNameDictionary = new Dictionary<AnimationEnum, string>();
        private Vector3 m_baseActorScale;
        private AnimationClip m_currentAnimationClip;
        private AnimationEnum m_currentAnimationEnum = AnimationEnum.None;
        private BaseActor m_baseActor;
        private Model m_model;
        private AnimationPlayer m_animationPlayer;
        private SkinningData m_skinningData;
    }

    public enum AnimationEnum
    {
        None,
        Idle,
        Walk,
        Run,
        Climb,
        Attack1,
        Attack2,
        Attack3,
        Miss,
        Block,
        Stagger,
        Die,
        Cast,
        Cheer
    }   



}
