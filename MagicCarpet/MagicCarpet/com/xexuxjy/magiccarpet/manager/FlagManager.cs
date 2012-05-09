using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.util;
using System.Diagnostics;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.manager
{
    public class FlagManager : EmptyGameObject
    {
        public FlagManager()
            : base(GameObjectType.manager)
        {
        }

        public Vector3 DefaultFlagDimensions
        {
            get { return new Vector3(1, 1, 1); }
        }


        public override void Initialize()
        {
            LoadContent();
            m_rotation = Matrix.CreateRotationX(MathUtil.SIMD_HALF_PI);
        }


        protected override void LoadContent()
        {
            int numSegments = 30;
            float width = DefaultFlagDimensions.X;
            float length = DefaultFlagDimensions.Y;
            m_effect = Globals.MCContentManager.GetEffect("Cloth");
            m_flagTexture = Globals.MCContentManager.GetTexture("Flag");
            ObjectBuilderUtil.BuildClothObject(numSegments, width, length, out m_dimensions, out m_flagVertexBuffer);

        }


        public void AddFlagForObject(GameObject owner, Vector3 flagPosition, Vector3 flagScale)
        {
            Debug.Assert(!m_flagData.ContainsKey(owner));
            FlagData flagData = new FlagData();
            flagData.Owner = owner;
            flagData.Position = flagPosition;
            flagData.Scale = flagScale;


            Vector3 min = flagPosition - new Vector3(1, 1, 0);
            Vector3 max = flagPosition + new Vector3(1, 1, 0);

            flagData.BoundingBox = new BoundingBox(min, max);

            m_flagData[owner] = flagData;
        }

        public void RemoveFlagForObject(GameObject owner)
        {
            Debug.Assert(m_flagData.ContainsKey(owner));
            m_flagData.Remove(owner);
        }

        public override void Draw(GameTime gameTime)
        {
            if (m_flagData.Count > 0)
            {
                BlendState oldState = Globals.Game.GraphicsDevice.BlendState;
                //Globals.Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

                m_effect.CurrentTechnique = m_effect.Techniques["DrawCloth"];

                Globals.MCContentManager.ApplyCommonEffectParameters(m_effect);
                Globals.GraphicsDevice.SetVertexBuffer(m_flagVertexBuffer);
                m_effect.Parameters["ClothTexture"].SetValue(m_flagTexture);

                float timeScalar = 4f;
                float frequency = 4f;
                m_flagMovementOffset += (timeScalar * m_dimensions.W * (float)gameTime.ElapsedGameTime.TotalSeconds);

                m_effect.Parameters["Frequency"].SetValue(frequency);
                m_effect.Parameters["Amplitude"].SetValue(m_dimensions.Y);
                m_effect.Parameters["ClothLength"].SetValue(m_dimensions.Z);


                m_effect.Parameters["ClothMovementOffset"].SetValue(m_flagMovementOffset);

                m_flagDataListSorted.Clear();
                foreach (FlagData flagData in m_flagData.Values)
                {
                    if (Globals.s_currentCameraFrustrum.Intersects(flagData.BoundingBox))
                    {
                        m_flagDataListSorted.Add(flagData);
                    }
                }
                m_flagDataListSorted.Sort(distanceComparator);


                foreach (FlagData flagData in m_flagDataListSorted)
                {
                    Vector3 startPosition = flagData.Position;
                    Matrix scaleMatrix = Matrix.CreateScale(flagData.Scale);
                    Matrix worldMatrix = scaleMatrix * m_rotation;
                    worldMatrix.Translation = flagData.Position;


                    m_effect.Parameters["WorldMatrix"].SetValue(worldMatrix);


                    int noTriangles = m_flagVertexBuffer.VertexCount / 3;


                    foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Globals.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
                    }

                }
                Globals.Game.GraphicsDevice.BlendState = oldState;
            }
        }


        // sorter for alpha draw?
        static Comparison<FlagData> distanceComparator = new Comparison<FlagData>(FlagDataSortPredicate);
        private static int FlagDataSortPredicate(FlagData lhs, FlagData rhs)
        {
            float lhsDist = (Globals.Camera.Position - lhs.Position).LengthSquared();
            float rhsDist = (Globals.Camera.Position - rhs.Position).LengthSquared();


            int result = (int)(rhsDist - lhsDist);
            return result;
        }


        private Matrix m_rotation;
        private Texture2D m_flagTexture;
        private Vector4 m_dimensions;
        private float m_flagMovementOffset;

        private VertexBuffer m_flagVertexBuffer;
        private Dictionary<GameObject, FlagData> m_flagData = new Dictionary<GameObject, FlagData>();
        private List<FlagData> m_flagDataListSorted = new List<FlagData>();

    }

    public class FlagData
    {
        public GameObject Owner;
        public Vector3 Position;
        public Vector3 Scale;
        public BoundingBox BoundingBox;
    }
}
