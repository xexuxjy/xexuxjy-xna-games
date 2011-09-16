using Microsoft.Xna.Framework;
using Dhpoware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ClipTerrainDemo
{
    public class ClipLevel
    {
        public ClipLevel(int scaleLevel, ClipLevelManager clipLevelManager)
        {
            m_scaleLevel = scaleLevel;
            m_clipLevelManager = clipLevelManager;
        }



        public void Draw(GraphicsDevice graphicsDevice, BoundingFrustum boundingFrustrum)
        {
            // cornera from top left so  go from -2 -> 1

            graphicsDevice.Indices = m_clipLevelManager.m_blockIndexBuffer;
            graphicsDevice.SetVertexBuffer(m_clipLevelManager.m_blockVertexBuffer);


            if (m_scaleLevel == 0)
            {
                DrawCore(boundingFrustrum);
            }
            //else
            {
                DrawRing(boundingFrustrum);
            }

        }

        public void DrawCore(BoundingFrustum boundingFrustrum)
        {
            DrawAtOffset(m_clipLevelManager.m_blockSize, m_clipLevelManager.m_blockSize, m_scaleLevel, boundingFrustrum, false);
            DrawAtOffset((m_clipLevelManager.m_blockSize * 2) - 1, m_clipLevelManager.m_blockSize, m_scaleLevel, boundingFrustrum, false);
            DrawAtOffset(m_clipLevelManager.m_blockSize, (m_clipLevelManager.m_blockSize * 2) - 1, m_scaleLevel, boundingFrustrum, false);
            DrawAtOffset((m_clipLevelManager.m_blockSize * 2) - 1, (m_clipLevelManager.m_blockSize * 2) - 1, m_scaleLevel, boundingFrustrum, false);
        }



        public void DrawRing(BoundingFrustum boundingFrustrum)
        {
            DrawAtOffset(0, 0, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(m_clipLevelManager.m_blockSize - 1, 0, m_scaleLevel, boundingFrustrum);
            DrawAtOffset((m_clipLevelManager.m_blockSize * 2), 0, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(((m_clipLevelManager.m_blockSize * 3) - 1), 0, m_scaleLevel, boundingFrustrum);

            DrawAtOffset(((m_clipLevelManager.m_blockSize * 3) - 1), m_clipLevelManager.m_blockSize - 1, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(((m_clipLevelManager.m_blockSize * 3) - 1), m_clipLevelManager.m_blockSize * 2, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(((m_clipLevelManager.m_blockSize * 3) - 1), ((m_clipLevelManager.m_blockSize * 3) - 1), m_scaleLevel, boundingFrustrum);

            DrawAtOffset(m_clipLevelManager.m_blockSize * 2, ((m_clipLevelManager.m_blockSize * 3) - 1), m_scaleLevel, boundingFrustrum);
            DrawAtOffset(m_clipLevelManager.m_blockSize - 1, ((m_clipLevelManager.m_blockSize * 3) - 1), m_scaleLevel, boundingFrustrum);
            DrawAtOffset(0, ((m_clipLevelManager.m_blockSize * 3) - 1), m_scaleLevel, boundingFrustrum);

            DrawAtOffset(0, m_clipLevelManager.m_blockSize * 2, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(0, m_clipLevelManager.m_blockSize - 1, m_scaleLevel, boundingFrustrum);

            DrawFixup(0, boundingFrustrum);
            DrawFixup(1, boundingFrustrum);
            DrawFixup(2, boundingFrustrum);
            DrawFixup(3, boundingFrustrum);


        }



        public void DrawFixup(int pos, BoundingFrustum boundingFrustrum)
        {
            int x = 0;
            int y = 0;
            int scaleFactor = (int)Math.Pow(2, m_scaleLevel);
            Matrix rotate = Matrix.Identity;
            BoundingBox box;
            Vector3 max = Vector3.Zero;
            switch (pos)
            {
                case (0):
                    {
                        x = ((m_clipLevelManager.m_blockSize * 2) - 2) * scaleFactor;
                        y = 0;
                        max = new Vector3(x + ((m_clipLevelManager.m_fixupOffset - 1) * scaleFactor), m_clipLevelManager.m_maxHeight, y + ((m_clipLevelManager.m_blockSize - 1) * scaleFactor));
                        break;
                    }
                case (1):
                    {
                        x = ((m_clipLevelManager.m_blockSize * 2) - 2) * scaleFactor;
                        y = ((m_clipLevelManager.m_blockSize * 3) - 1) * scaleFactor;
                        max = new Vector3(x + ((m_clipLevelManager.m_fixupOffset - 1) * scaleFactor), m_clipLevelManager.m_maxHeight, y + ((m_clipLevelManager.m_blockSize - 1) * scaleFactor));
                        break;
                    }
                case (2):
                    {
                        x = ((m_clipLevelManager.m_blockSize * 3) - 1) * scaleFactor;
                        y = ((m_clipLevelManager.m_blockSize * 2) - 2) * scaleFactor;
                        max = new Vector3(x + ((m_clipLevelManager.m_blockSize - 1) * scaleFactor), m_clipLevelManager.m_maxHeight, y + ((m_clipLevelManager.m_fixupOffset - 1) * scaleFactor));
                        break;
                    }
                case (3):
                    {
                        x = 0;
                        y = ((m_clipLevelManager.m_blockSize * 2) - 2) * scaleFactor;
                        max = new Vector3(x + ((m_clipLevelManager.m_blockSize - 1) * scaleFactor), m_clipLevelManager.m_maxHeight, y + ((m_clipLevelManager.m_fixupOffset - 1) * scaleFactor));
                        break;
                    }
            }
            Vector3 min = new Vector3(x, -m_clipLevelManager.m_maxHeight, y);
            //min = Vector3.TransformNormal(min,rotate);
            //max = Vector3.TransformNormal(max, rotate);

            min += m_offset;
            max += m_offset;
            //x = 0;
            //y = 0;
            //rotate = Matrix.Identity;

            box = new BoundingBox(min, max);

            InternalDraw(ref box, boundingFrustrum, ref rotate, m_clipLevelManager.m_blockSize * 3, m_clipLevelManager.m_fixupIndexBufferH.IndexCount);
        }


        public void DrawAtOffset(int x, int y, int scaleLevel, BoundingFrustum boundingFrustrum)
        {
            DrawAtOffset(x, y, scaleLevel, boundingFrustrum, true);
        }

        public void DrawAtOffset(int x, int y, int scaleLevel, BoundingFrustum boundingFrustrum, bool applyFixups)
        {
            int scaleFactor = (int)Math.Pow(2, scaleLevel);

            x *= scaleFactor;
            y *= scaleFactor;

            x += (int)m_offset.X;
            y += (int)m_offset.Z;

            BoundingBox box = new BoundingBox(new Vector3(x, -m_clipLevelManager.m_maxHeight, y), new Vector3(x + ((m_clipLevelManager.m_blockSize - 1) * scaleFactor), m_clipLevelManager.m_maxHeight, y + ((m_clipLevelManager.m_blockSize - 1) * scaleFactor)));
            Matrix worldMatrix = Matrix.Identity;
            InternalDraw(ref box, boundingFrustrum, ref worldMatrix, m_clipLevelManager.m_blockVertexBuffer.VertexCount, m_clipLevelManager.m_blockIndexBuffer.IndexCount);
        }


        private void InternalDraw(ref BoundingBox boundingBox, BoundingFrustum boundingFrustrum, ref Matrix world, int numVertices, int numIndices)
        {
            if (boundingFrustrum.Intersects(boundingBox))
            {
                int scaleFactor = (int)Math.Pow(2, m_scaleLevel);
                m_clipLevelManager.m_effect.Parameters["WorldViewProjMatrix"].SetValue(world * m_clipLevelManager.m_camera.ViewMatrix * m_clipLevelManager.m_camera.ProjectionMatrix);
                m_clipLevelManager.m_effect.Parameters["ScaleFactor"].SetValue(new Vector4(scaleFactor, scaleFactor, boundingBox.Min.X, boundingBox.Min.Z));
                m_clipLevelManager.m_effect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(1.0f / m_clipLevelManager.m_heightMapTexture.Width, 1.0f / m_clipLevelManager.m_heightMapTexture.Height, 0, 0));

                foreach (EffectPass pass in m_clipLevelManager.m_effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    m_clipLevelManager.Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numIndices / 3);
                }
            }
        }




        private ClipLevelManager m_clipLevelManager;
        private int m_scaleLevel;
        private Vector3 m_offset;


    }
}
