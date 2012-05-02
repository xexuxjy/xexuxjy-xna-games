using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.util;
using System.Diagnostics;

namespace com.xexuxjy.magiccarpet.manager
{
    public class FlagManager : EmptyGameObject
    {
        public FlagManager()
            : base(GameObjectType.manager)
        {
        }

        protected override void LoadContent()
        {
            int numSegments = 30;
            float width = 1;
            float length = 2;
            m_flagEffect = Globals.MCContentManager.GetEffect("OwnerColour");
            ObjectBuilderUtil.BuildClothObject(numSegments, width, length, out m_dimensions, out m_flagVertexBuffer);

        }


        public void AddFlagForObject(GameObject owner, Vector3 flagPosition, Vector3 flagScale)
        {
            Debug.Assert(!m_flagData.ContainsKey(owner));
            FlagData flagData = new FlagData();
            flagData.Owner = owner;
            flagData.Position = flagPosition;
            flagData.Scale = flagScale;
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
                foreach (FlagData flagData in m_flagData.Values)
                {
                    if (Globals.s_currentCameraFrustrum.Intersects(flagData.BoundingBox))
                    {
                        Globals.MCContentManager.ApplyCommonEffectParameters(m_flagEffect);

                        Globals.GraphicsDevice.SetVertexBuffer(m_flagVertexBuffer);

                        Vector3 startPosition = flagData.Position;
                        Matrix scaleMatrix = Matrix.CreateScale(flagData.Scale);
                        Matrix worldMatrix = scaleMatrix * m_rotation;
                        worldMatrix.Translation = flagData.Position;


                        m_flagEffect.Parameters["WorldMatrix"].SetValue(worldMatrix);
                        m_flagEffect.Parameters["ClothTexture"].SetValue(m_flagTexture);

                        float timeScalar = 4f;
                        float frequency = 4f;
                        m_flagMovementOffset += (timeScalar * m_dimensions.W * (float)gameTime.ElapsedGameTime.TotalSeconds);

                        m_flagEffect.Parameters["Frequency"].SetValue(frequency);
                        m_flagEffect.Parameters["Amplitude"].SetValue(m_dimensions.Y);
                        m_flagEffect.Parameters["ClothLength"].SetValue(m_dimensions.Z);


                        m_flagEffect.Parameters["ClothMovementOffset"].SetValue(m_flagMovementOffset);

                        m_flagEffect.Parameters["OwnerColor"].SetValue(flagData.Owner.BadgeColor.ToVector3());


                        int noTriangles = m_flagVertexBuffer.VertexCount / 3;

                        m_flagEffect.CurrentTechnique = m_flagEffect.Techniques["DrawCloth"];

                        foreach (EffectPass pass in m_flagEffect.CurrentTechnique.Passes)
                        {
                            int noVertices = m_flagVertexBuffer.VertexCount;
                            pass.Apply();
                            Globals.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
                        }


                    }

                }
            }
        }


        private Matrix m_rotation;
        private Texture2D m_flagTexture;
        private Effect m_flagEffect;
        private Vector4 m_dimensions;
        private float m_flagMovementOffset;

        private VertexBuffer m_flagVertexBuffer;
        private Dictionary<GameObject, FlagData> m_flagData = new Dictionary<GameObject, FlagData>();

    }

    public struct FlagData
    {
        public GameObject Owner;
        public Vector3 Position;
        public Vector3 Scale;
        public BoundingBox BoundingBox;
    }
}
