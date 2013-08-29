﻿
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
            BoundingBox bb = new BoundingBox();
            foreach (ModelMesh mesh in m_model.Meshes)
            {
                CalculateBoundingBox(mesh, ref bb);
            }
            Vector3 diff = bb.Max - bb.Min;
            float maxSpan = Math.Max(diff.X, Math.Max(diff.Y, diff.Z));
            //BoundingSphere actorBs = m_model.Meshes[0].BoundingSphere;
            m_baseActorScale = new Vector3(1f / maxSpan);

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
                foreach (Effect effect in mesh.Effects)
                {
                    SkinnedEffect skinnedEffect = effect as SkinnedEffect;
                    if (skinnedEffect != null)
                    {
                        skinnedEffect.World = boneWorld;
                        skinnedEffect.SetBoneTransforms(bones);

                        skinnedEffect.View = camera.ViewMatrix;
                        skinnedEffect.Projection = camera.ProjectionMatrix;

                        skinnedEffect.EnableDefaultLighting();

                        skinnedEffect.SpecularColor = new Vector3(0.25f);
                        skinnedEffect.SpecularPower = 16;
                    }
                    else
                    {
                        BasicEffect basicEffect = effect as BasicEffect;
                        if (basicEffect != null)
                        {
                            basicEffect.World = boneWorld;
                            //basicEffect.SetBoneTransforms(bones);

                            basicEffect.View = camera.ViewMatrix;
                            basicEffect.Projection = camera.ProjectionMatrix;

                            basicEffect.EnableDefaultLighting();

                            basicEffect.SpecularColor = new Vector3(0.25f);
                            basicEffect.SpecularPower = 16;

                        }
                    }
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

        public static void CalculateBoundingBox(ModelMesh mm, ref BoundingBox bb)
        {
            bb = new BoundingBox();
            bool first = true;
            Matrix x = Matrix.Identity;
            ModelBone mb = mm.ParentBone;
            while (mb != null)
            {
                x = x * mb.Transform;
                mb = mb.Parent;
            }


            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);

            foreach (ModelMeshPart part in mm.MeshParts)
            {
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                //VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                Vector3[] vertexData = new Vector3[part.NumVertices];
                //int num = (part.NumVertices - part.VertexOffset);
                int num = part.NumVertices;

                //GetData(offsetFromStartOfVertexBufferInBytes, arrayOfVector3, 0, arrayOfVector3.Length, sizeOfEachVertexInBytes);

                part.VertexBuffer.GetData(0, vertexData, 0, num, stride);
                //part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, num, stride);

                // Find minimum and maximum xyz values for this mesh part
                //Vector3 vertPosition = new Vector3();

                for (int i = 0; i < vertexData.Length; i++)
                {
                    Vector3 vertPosition = vertexData[i];
                    //vertPosition.X = vertexData[i];
                    //vertPosition.Y = vertexData[i + 1];
                    //vertPosition.Z = vertexData[i + 2];

                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                    i += stride;
                }
            }

            // transform by mesh bone matrix
            //meshMin = Vector3.Transform(meshMin, meshTransform);
            //meshMax = Vector3.Transform(meshMax, meshTransform);

            // Create the bounding box
            //BoundingBox box = new BoundingBox(meshMin, meshMax);

            BoundingBox newbox  = new BoundingBox(meshMin, meshMax);
            bb = MergeBoxes(bb, newbox);
            //return box;
        }

        public static BoundingBox MergeBoxes(BoundingBox one, BoundingBox two)
        {
            Vector3 min = one.Min;
            Vector3 max = one.Max;

            min.X = Math.Min(one.Min.X, two.Min.X);
            min.Y = Math.Min(one.Min.Y, two.Min.Y);
            min.Z = Math.Min(one.Min.Z, two.Min.Z);

            max.X = Math.Max(one.Max.X, two.Max.X);
            max.Y = Math.Max(one.Max.Y, two.Max.Y);
            max.Z = Math.Max(one.Max.Z, two.Max.Z);

            return new BoundingBox(min, max);
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
