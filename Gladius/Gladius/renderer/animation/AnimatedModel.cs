
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public AnimatedModel(float desiredScale=1f)
        {
            ModelRotation = Quaternion.Identity;
            m_desiredScale = desiredScale;
        }

        public String ModelName
        {
            get;
            set;
        }

        public Quaternion ModelRotation
        {
            get;
            set;
        }


        public void Update(GameTime gameTime)
        {
            m_animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            if (m_currentAnimationEnum == AnimationEnum.Attack1)
            {
                int ibreak = 0;
            }

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

        public void SetMeshActive(String meshName, bool active)
        {
            if (m_meshActiveDictionary.ContainsKey(meshName))
            {
                m_meshActiveDictionary[meshName] = active;
            }
        }


        public void LoadContent(ContentManager content)
        {
            // Load the model.
            m_model = content.Load<Model>(ModelName);
            BoundingBox bb = new BoundingBox();
            foreach (ModelMesh mesh in m_model.Meshes)
            {
                int ibreak = 0;
                GraphicsHelper.CalculateBoundingBox(mesh, ref bb);
                m_meshActiveDictionary[mesh.Name] = true;
                
            }
            Vector3 diff = bb.Max - bb.Min;
            float maxSpan = Math.Max(diff.X, Math.Max(diff.Y, diff.Z));
            //BoundingSphere actorBs = m_model.Meshes[0].BoundingSphere;
            m_baseActorScale = new Vector3(m_desiredScale/ maxSpan);

            BoundingBox = new BoundingBox(bb.Min * m_baseActorScale, bb.Max * m_baseActorScale);

            //m_baseActorScale = Vector3.One;

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

            m_clipNameDictionary[AnimationEnum.Idle] = "Idle";
            m_clipNameDictionary[AnimationEnum.Walk] = "Walk";
            m_clipNameDictionary[AnimationEnum.Attack1] = "Attack1";
            m_clipNameDictionary[AnimationEnum.Attack2] = "Attack2";
            m_clipNameDictionary[AnimationEnum.Attack3] = "Attack3";
            m_clipNameDictionary[AnimationEnum.BowShot] = "BowShot";
            m_clipNameDictionary[AnimationEnum.Stagger] = "Hit1";
            m_clipNameDictionary[AnimationEnum.Die] = "Death";
            m_clipNameDictionary[AnimationEnum.Block] = "Block";
            m_clipNameDictionary[AnimationEnum.Cast] = "Cast";

        }

        public Quaternion ActorRotation
        {
            get;
            set;
        }

        public Vector3 ActorPosition
        {
            get;
            set;
        }

        public void Draw(GraphicsDevice device, ICamera camera, GameTime gameTime)
        {
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;

            //Matrix[] bones = m_animationPlayer.SkinTransforms;
            Matrix[] bones = m_animationPlayer.SkinTransforms;


            //Quaternion q = Quaternion.C


            Matrix world = Matrix.CreateScale(m_baseActorScale) * Matrix.CreateFromQuaternion(ActorRotation * ModelRotation) * Matrix.CreateTranslation(ActorPosition);

            

            // Render the skinned mesh.
            foreach (ModelMesh mesh in m_model.Meshes)
            {
                // only draw those that are active..
                if (!m_meshActiveDictionary[mesh.Name])
                {
                    continue;
                }

                //Matrix boneWorld = bones[mesh.ParentBone.Index] * world;
                Matrix boneWorld = world;
                foreach (Effect effect in mesh.Effects)
                {
                    SkinnedEffect skinnedEffect = effect as SkinnedEffect;
                    if (skinnedEffect != null)
                    {
                        skinnedEffect.World = boneWorld;
                        //skinnedEffect.SetBoneTransforms(bones);
                        skinnedEffect.SetBoneTransforms(m_animationPlayer.SkinTransforms);

                        skinnedEffect.View = camera.View;
                        skinnedEffect.Projection = camera.Projection;

                        ApplyLighting(skinnedEffect);
                    }
                    else
                    {
                        BasicEffect basicEffect = effect as BasicEffect;
                        if (basicEffect != null)
                        {
                            basicEffect.World = boneWorld;
                            //basicEffect.SetBoneTransforms(bones);

                            basicEffect.View = camera.View;
                            basicEffect.Projection = camera.Projection;

                            basicEffect.EnableDefaultLighting();

                            //basicEffect.SpecularColor = new Vector3(0.25f);
                            //basicEffect.SpecularPower = 16;

                        }
                    }
                }

                mesh.Draw();
            }
        }

        public void PlayAnimation(AnimationEnum animationEnum,bool loopClip = true)
        {
            if (m_currentAnimationEnum == AnimationEnum.Attack1)
            {
                int ibreak = 0;
            }


            if (m_currentAnimationEnum != animationEnum)
            {
                if (animationEnum == AnimationEnum.Stagger)
                {
                    int ibreak = 0;
                }

                String clipName;
                if (m_clipNameDictionary.TryGetValue(animationEnum, out clipName))
                {
                    m_currentAnimationEnum = animationEnum;
                    m_currentAnimationClip = m_skinningData.AnimationClips[clipName];
                    if (m_currentAnimationClip != null)
                    {
                        Globals.EventLogger.LogEvent(EventTypes.Animation, String.Format("PlayAnimation [{0}] [{1}] [{2}][{3}]", DebugName, animationEnum, clipName, loopClip));
                        m_animationPlayer.StartClip(m_currentAnimationClip, loopClip);
                        if (OnAnimationStarted != null)
                        {
                            OnAnimationStarted(m_currentAnimationEnum);
                        }
                    }
                }
                else
                {
                    Globals.EventLogger.LogEvent(EventTypes.Animation, String.Format("PlayAnimation FailedNoMatch [{0}] [{1}]", DebugName, animationEnum));
                }
            }
        }

        public bool FindMatrixForBone(String boneName, out Matrix result)
        {
        //ModelBone parentbone = model.Model.Bones.Where(x => x.Name == item.Key).Single();
        //UpdateModelAnimation(item.Value, gameTime, bones[model.SkinningData.SkeletonHierarchy[parentbone.Index - 2]]);

            ModelBone resultBone;
            if (m_model.Bones.TryGetValue(boneName, out resultBone))
            {
                result = m_animationPlayer.BoneTransforms[resultBone.Index];
                return true;
            }
            else
            {
                result = Matrix.Identity;
                return false;
            }
        }

        public static void ApplyLighting(SkinnedEffect effect)
        {
            effect.DirectionalLight0.Enabled = true; // turn on the lighting subsystem.
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f);
            effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);  // coming along the x-axis
            effect.DirectionalLight0.SpecularColor = new Vector3(0.2f);

            effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
            effect.EmissiveColor = new Vector3(1);
        }

        public String DebugName
        {
            get;
            set;
        }

        
        public delegate void AnimationStarted(AnimationEnum anim);
        public delegate void AnimationStopped(AnimationEnum anim);
        public event AnimationStarted OnAnimationStarted;
        public event AnimationStopped OnAnimationStopped;

        public BoundingBox BoundingBox
        {
            get;
            set;
        }

        public void RegisterEvent(AnimationEnum animationEnum,String eventName,CpuSkinningDataTypes.AnimationPlayer.EventCallback callback)
        {
            m_animationPlayer.RegisteredEvents[animationEnum.ToString()].Add(eventName, callback);
        }

        public void UnregisterEvent(AnimationEnum animationEnum, String eventName)
        {
            m_animationPlayer.RegisteredEvents[animationEnum.ToString()].Remove(eventName);
        }


        private Dictionary<String, bool> m_meshActiveDictionary = new Dictionary<String, bool>();

        private Dictionary<AnimationEnum, String> m_clipNameDictionary = new Dictionary<AnimationEnum, string>();

        private float m_desiredScale;
        private Vector3 m_baseActorScale;
        private AnimationClip m_currentAnimationClip;
        private AnimationEnum m_currentAnimationEnum = AnimationEnum.None;
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
        BowShot,
        Miss,
        Block,
        Stagger,
        Die,
        Cast,
        Cheer
    }   

/*
 * w_shoes_01
w_pants_01
w_hand_01
w_helmet_01
w_shoulder_01
w_body_01
w_head_01

bow_01
shield_01
sword_01
 * */

}
