using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.util;
using System.Diagnostics;
using BulletXNA;
using BulletXNA.LinearMath;

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
            get { return new Vector3(1, 0.1f, 1); }
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
            float length = DefaultFlagDimensions.Z;
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


            Vector3 min = flagPosition - DefaultFlagDimensions / 2f;
            Vector3 max = flagPosition + DefaultFlagDimensions / 2f;

            flagData.BoundingSphere = new BoundingSphere(flagPosition,(DefaultFlagDimensions.X/2*flagScale.X));

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
                m_effect.Parameters["Texture"].SetValue(m_flagTexture);

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
                    if (Globals.s_currentCameraFrustrum.Contains(flagData.BoundingSphere) != ContainmentType.Disjoint)
                    {
                        m_flagDataListSorted.Add(flagData);
                    }
                    else
                    {
                        int ibreak = 0;
                    }
                }
                //m_flagDataListSorted.Sort(distanceComparator);


                foreach (FlagData flagData in m_flagDataListSorted)
                {
                    Vector3 startPosition = flagData.Position;
                    Matrix scaleMatrix = Matrix.CreateScale(flagData.Scale);
                    Matrix worldMatrix = scaleMatrix * m_rotation * Matrix.CreateTranslation(flagData.Position);
                    //worldMatrix.Translation = flagData.Position;

                    Vector3 scaled = DefaultFlagDimensions;

                    IndexedVector3 min = -(scaled / 2f);
                    IndexedVector3 max = scaled / 2f;

                    IndexedVector3 colour = new IndexedVector3(0, 0, 1);
                    IndexedMatrix m = new IndexedMatrix(worldMatrix);
                    //m = IndexedMatrix.Identity;
                    m._origin = flagData.Position;

                    // already been translated
                    //m._origin = IndexedVector3.Zero;
                    Globals.DebugDraw.DrawBox(ref min, ref max, ref m, ref colour);

                    m_effect.Parameters["WorldMatrix"].SetValue(worldMatrix);


                    int noTriangles = m_flagVertexBuffer.VertexCount / 3;


                    foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Globals.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
                    }

                }

                Globals.DebugDraw.DrawText(String.Format("FlagManager visible[{0}] total[{1}]", m_flagDataListSorted.Count, m_flagData.Count), new IndexedVector3(10, 70, 0), new IndexedVector3(1, 1, 1));

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
        public BoundingSphere BoundingSphere;
    }
}
